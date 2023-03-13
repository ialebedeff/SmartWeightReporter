namespace SmartWeight.MemoryBase
{
    public class UpdateHistoryStore
    { 
        public int Id { get; set; }
        public string Build { get; set; } = null!;
        public bool IsSucces => UpdateSteps.All(step => step.IsSuccess);
        public List<UpdateStepStore> UpdateSteps { get; set; } = new();
    }

    public class UpdateStepStore
    { 
        public int Id { get; set; }
        public string State { get; set; } = null!;
        public string? ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public UpdateHistoryStore UpdateHistoryStore { get; set; } = null!;
    }
}