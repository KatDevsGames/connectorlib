namespace ConnectorLib
{
    /// <remarks>
    /// MAKE SURE THIS ENUM EXACTLY MATCHES THE CORRESPONDING ONE IN BitRaceCommon
    /// THE ONE IN BitRaceCommon IS THE CANONICAL COPY DEFINED AS CORRECT
    /// </remarks>
    public enum ConnectorType : byte
    {
        SNESConnector = 0x00,
        NESConnector = 0x01,
        GenesisConnector = 0x02,
        N64Connector = 0x03,
        GBAConnector = 0x04
    }
}
