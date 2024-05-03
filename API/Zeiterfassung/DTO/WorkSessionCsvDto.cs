namespace Zeiterfassung.DTO;

public class WorkSessionCsvDto
{
    public int WorkSessionId { get; set; }
    public double WorkingHours { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public string LocationDescription { get; set; }
    public string ProjectName { get; set; }
}
