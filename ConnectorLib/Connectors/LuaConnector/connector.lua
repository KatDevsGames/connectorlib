local CLIENT_HOST = "localhost"
local socket = require("socket.core")
local json = require("json")
local tcp
local frameCounter = 0
local effectQueue = {}
local lastKeepalive = 0
local wasReset = false

local function BitXOR(a,b)--Bitwise XOR
    local p,c=1,0
    while a>0 and b>0 do
        local ra,rb=a%2,b%2
        if ra~=rb then c=c+p end
        a,b,p=(a-ra)/2,(b-rb)/2,p*2
    end
    if a<b then a=b end
    while a>0 do
        local ra=a%2
        if ra>0 then c=c+p end
        a,p=(a-ra)/2,p*2
    end
    return c
end

local function BitOR(a,b)--Bitwise OR
    local p,c=1,0
    while a+b>0 do
        local ra,rb=a%2,b%2
        if ra+rb>0 then c=c+p end
        a,b,p=(a-ra)/2,(b-rb)/2,p*2
    end
    return c
end

local function BitNOT(n)--Bitwise NOT
    local p,c=1,0
    while n>0 do
        local r=n%2
        if r<1 then c=c+p end
        n,p=(n-r)/2,p*2
    end
    return c
end

local function BitAND(a,b)--Bitwise AND
    local p,c=1,0
    while a>0 and b>0 do
        local ra,rb=a%2,b%2
        if ra+rb>1 then c=c+p end
        a,b,p=(a-ra)/2,(b-rb)/2,p*2
    end
    return c
end

function tick()
	processBuffer()
	checkKeepalive()
	frameCounter = frameCounter + 1
end

function checkKeepalive()
	if (lastKeepalive+(5*60))<frameCounter then
		emu.message("Connection lost. Attempting to reestablish...")
		wasReset = true
		lastKeepalive = frameCounter
		buffPtr = 0
		tcp = socket.connect(CLIENT_HOST,43884)
		if tcp then tcp:settimeout(0) end
	end
end

function sendBlock(block)
	local outBuf = json.encode(block)..string.char(0);
	if not tcp:send(outBuf) then
		emu.message("unable to update server")
	end
end

function processBlock(block)
	lastKeepalive = frameCounter
	
	if(not block) then return end
	local commandType = block["type"]
	
	local result = {}
	result["id"] = block["id"]
	result["stamp"] = os.time(os.date("!*t"))
	result["type"] = commandType
	result["message"] = ""
	result["address"] = block["address"]
	result["value"] = block["value"]
	
	if commandType==0x00 then --read byte
		result["value"]=memory.readbyte(block["address"])
	elseif commandType==0x01 then --read byte
		result["value"]=memory.readword(block["address"])
	elseif commandType==0x10 then --write byte
		memory.writebyte(block["address"],block["value"])
	elseif commandType==0x11 then --write word
		memory.writeword(block["address"],block["value"])
	elseif commandType==0x20 then --safe bit flip (atomic)
		local old=memory.readbyte(block["address"])
		memory.writebyte(address,BitOR(old,block["value"]))
		block["value"]=old
	elseif commandType==0x21 then --safe bit flip (atomic)
		local old=memory.readbyte(block["address"])
		memory.writebyte(address,BitAND(old,BitNOT(block["value"])))
		block["value"]=old
	elseif commandType==0xF0 then
		emu.message(block["message"])
	elseif commandType==0xFF then
		-- do nothing
	else
		emu.message("Unknown block type received.")
	end
	
	sendBlock(result)
end

function processBuffer()
	if not tcp then return end
	
	local buffer = ""
	
	while true do
		local nb, err = tcp:receive(1)
		if not nb then
			if err ~= "timeout" then
				emu.message("Connection died: " .. err)
				lastKeepalive = 0
				buffer = ""
				return
			end
			break
		end
		
		if wasReset then
			emu.message("The connection was reestablished.")
			wasReset = false
			buffer=""
		end
		
		buffer=buffer..nb
		if string.byte(nb)==0 then
			local unpacked = json.decode(buffer)
			processBlock(unpacked)
			if(not unpacked) then emu.message("Failed to unpack block " .. buffer) end
			buffer=""
		end
	end
end

if emu.emulating() then
	tcp = socket.connect(CLIENT_HOST,43884)
	if not tcp then emu.message("failed to open socket") return end
	tcp:settimeout(0)
	emu.message("The connection was established.")
	emu.registerbefore(tick)
else
	emu.message("failed to start - emulator not running")
end
