using System.Collections.Generic;

public class StatusInfo {
    public string id;
    public string header;
    public string effect;
    public string curedBy;
}

public class TooltipConfigStatus {
    public string effect;
    public string curedBy;
}

public class Status {
    public List<StatusInfo> types;
    public TooltipConfigStatus tooltipConfig;
}

public class GameConfig {
    public static Status status;
    public static Dictionary<string, string> locized;
}