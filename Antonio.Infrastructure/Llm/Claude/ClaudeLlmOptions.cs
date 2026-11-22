using System;
using System.ComponentModel.DataAnnotations;

namespace Antonio.Infrastructure.Llm.Claude;

public class ClaudeLlmOptions : ILlmOptions
{
    public static string SectionName => "Antonio:Llm:Claude";
    
   [Required] public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "claude-sonnet-5-5";
    public string BaseUrl { get; set; } = "https://api.anthropic.com/";

}
