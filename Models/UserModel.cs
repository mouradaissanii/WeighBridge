namespace WeighBridge.Models
{
    public enum UserRole { Agent, Commercial, Administrator }
    public enum Shift    { Morning, Afternoon, Night }

    public class UserModel
    {
        public string   OperatorId    { get; set; } = string.Empty;
        public string   FullName      { get; set; } = string.Empty;
        public string   Initials      { get; set; } = string.Empty;
        public UserRole Role          { get; set; }
        public Shift    Shift         { get; set; }
        public string   WeighbridgeId { get; set; } = "WB-01";
        public bool IsActive { get; set; } = true;

        // ── Computed display properties ──
        public string RoleDisplayName => Role switch
        {
            UserRole.Administrator => "Administrator",
            UserRole.Commercial     => "Commercial",
            _                       => "Agent"
        };

        public string ShiftDisplayName => Shift switch
        {
            Shift.Afternoon => "Afternoon Shift",
            Shift.Night     => "Night Shift",
            _               => "Morning Shift"
        };

        // Color for role badge — used in sidebar + dashboard
        public string RoleBadgeColor => Role switch
        {
            UserRole.Administrator => "#E8001D",
            UserRole.Commercial     => "#2ECC71",
            _                       => "#C9A84C"
        };

        public string StationInfo =>
            $"{ShiftDisplayName} · {WeighbridgeId} · Algiers DZA";
    }
}
