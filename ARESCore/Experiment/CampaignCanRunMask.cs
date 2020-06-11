namespace ARESCore.Experiment
{
  public enum CampaignCanRunMask
  {
    // Valid values should be "OR"ed ( | ) and Invalid values should be "AND"ed ( & )

    // LSB
    Planner = 0b1,
    NoPlanner = 0b11111110,
    // 7th bit
    Experiments = 0b10,
    NoExperiments = 0b11111101,
    // 6th bit
    ValidExperimentScript = 0b100,
    InvalidExperimentScript = 0b11111011,
    // 5th bit
    PlanResults = 0b1000,
    NoPlanResults = 0b11110111,
    // 4th bit
    ValidInterScript = 0b10000,
    InvalidInterScript = 0b11101111,
    // 3rd bit
    ValidCloseScript = 0b100000,
    InvalidCloseScript = 0b11011111,
    // 2nd bit
    CampaignPending = 0b1000000,
    CampaignRunning = 0b10111111,
    // MSB

  }
}
