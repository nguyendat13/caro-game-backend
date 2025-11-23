namespace backend.Models.Enums
{
    public enum TournamentFormat
    {
        SingleElimination = 1,   // Loại trực tiếp
        DoubleElimination = 2,   // Nhánh thắng - thua
        RoundRobin = 3,          // Vòng tròn
        Swiss = 4,               // Hệ Thụy Sĩ
        GroupStage = 5,          // Vòng bảng
        Knockout = 6,            // Playoff
    }
}
