local CLIENT_HOST = "localhost"
local luasocket = require("socket.core")
local socket = luasocket.tcp()
local json = require("json")
local frameCounter = 0
local effectQueue = {}
local lastKeepalive = 0
local wasReset = false
local busDomain = nil

function tick()
	processBuffer()
	checkKeepalive()
	frameCounter = frameCounter + 1
end

function sendMessage(message)
	print(message)
	gui.addmessage(message)
end

function checkKeepalive()
	if ((lastKeepalive+(5*60))<frameCounter) then
		wasReset = true
		buffPtr = 0
		sendMessage("Connection lost. Attempting to reestablish...")
		socket:close()
		socket = luasocket.tcp()
		if socket:connect(CLIENT_HOST,43884)==1 then socket:settimeout(0) end
	end
end

function sendBlock(block)
	local outBuf = json.encode(block)..string.char(0);
	if not socket:send(outBuf) then
		sendMessage("unable to update server")
	end
end

function processBlock(block)
	lastKeepalive = frameCounter
	
	if(not block) then return end
	local commandType = block["type"]
	
	--if not (block.type==255) then print(block) end
	
	if block["domain"]=="ROM" then block["domain"]="CARTROM" end
	if block["domain"]=="SRAM" then block["domain"]="CARTRAM" end
	
	local domain
	local domainAddress
	if busDomain then
		domain=busDomain
		domainAddress=block["address"]
	else
		domain=block["domain"]
		domainAddress=block["domainAddress"]
	end
	
	local result = {}
	result["id"] = block["id"]
	result["stamp"] = os.time(os.date("!*t"))
	result["type"] = commandType
	result["message"] = ""
	result["address"] = block["address"]
	result["domain"] = block["domain"]
	result["domainAddress"] = block["domainAddress"]
	result["value"] = block["value"]
	
	if commandType==0x00 then --read byte
		result["value"]=memory.read_u8(domainAddress,domain)
	elseif commandType==0x01 then --read ushort
		result["value"]=memory.read_u16_le(domainAddress,domain)
		
	elseif commandType==0x10 then --write byte
		memory.write_u8(domainAddress,block["value"],domain)
	elseif commandType==0x11 then --write ushort
		memory.write_u16_le(domainAddress,block["value"],domain)
		
	elseif commandType==0x20 then --safe bit flip (atomic)
		local old=memory.read_u8(domainAddress,domain)
		memory.write_u8(domainAddress,bit.bor(old,block["value"]),domain)
		block["value"]=old
	elseif commandType==0x21 then --safe bit flip (atomic)
		local old=memory.read_u8(block["address"])
		memory.write_u8(domainAddress,bit.band(old,bit.bnot(block["value"])),domain)
		block["value"]=old
	elseif commandType==0xF0 then
		sendMessage(block["message"])
	elseif commandType==0xFF then
		-- do nothing
	else
		sendMessage("Unknown block type received.")
	end
	
	sendBlock(result)
end

function processBuffer()
	if not socket then return end
	
	local buffer = ""
	
	while true do
		local nb, err = socket:receive(1)
		if not nb then
			if err ~= "timeout" then
				sendMessage("Connection died: " .. err)
				lastKeepalive = 0
				buffer = ""
				return
			end
			break
		end
		
		if wasReset then
			sendMessage("The connection was reestablished.")
			wasReset = false
			buffer=""
		end
		
		buffer=buffer..nb
		if string.byte(nb)==0 then
			local unpacked = json.decode(buffer)
			processBlock(unpacked)
			if(not unpacked) then sendMessage("Failed to unpack block " .. buffer) end
			buffer=""
		end
	end
end

for key, value in pairs(memory.getmemorydomainlist()) do
  if value=="System Bus" then
    busDomain=value
  end
end
event.unregisterbyname("connectorlib")
console.clear()
if not (emu.getsystemid()=="NULL") then
	if not (socket:connect(CLIENT_HOST,43884)==1) then
		sendMessage("failed to open socket")
	else
		socket:settimeout(0)
		sendMessage("The connection was established.")
		event.onframestart(tick,"connectorlib")
	end
else
	sendMessage("failed to start - emulator not running")
end
