using System.Text.RegularExpressions;

namespace AIChatbot.Services;

public class OrderStatusService
{
    private readonly ILogger<OrderStatusService> _logger;
    private readonly Dictionary<string, OrderStatus> _orderStatuses;

    public OrderStatusService(ILogger<OrderStatusService> logger)
    {
        _logger = logger;
        _orderStatuses = InitializeMockOrders();
    }

    public bool IsStatusInquiry(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        var lowerMessage = message.ToLowerInvariant();
        var statusKeywords = new[] { "status", "check", "track", "where is", "progress", "update", "application status", "request status", "my application", "my request", "what is the status" };
        
        return statusKeywords.Any(keyword => lowerMessage.Contains(keyword));
    }

    public string? ExtractIdNumber(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        // Saudi National ID patterns (10 digits)
        // Pattern 1: 10-digit ID number
        var pattern1 = @"\b(\d{10})\b";
        var match1 = Regex.Match(message, pattern1);
        if (match1.Success)
        {
            return match1.Groups[1].Value;
        }

        // Pattern 2: ID with spaces or dashes (e.g., 123-456-7890 or 123 456 7890)
        var pattern2 = @"\b(\d{3}[-.\s]?\d{3}[-.\s]?\d{4})\b";
        var match2 = Regex.Match(message, pattern2);
        if (match2.Success)
        {
            return match2.Groups[1].Value.Replace("-", "").Replace(".", "").Replace(" ", "");
        }

        // Pattern 3: Arabic numerals (10 digits)
        var pattern3 = @"[\u0660-\u0669\u06F0-\u06F9]{10}";
        var match3 = Regex.Match(message, pattern3);
        if (match3.Success)
        {
            // Convert Arabic numerals to Western numerals
            var arabicToWestern = match3.Value
                .Replace('\u0660', '0').Replace('\u0661', '1').Replace('\u0662', '2')
                .Replace('\u0663', '3').Replace('\u0664', '4').Replace('\u0665', '5')
                .Replace('\u0666', '6').Replace('\u0667', '7').Replace('\u0668', '8').Replace('\u0669', '9')
                .Replace('\u06F0', '0').Replace('\u06F1', '1').Replace('\u06F2', '2')
                .Replace('\u06F3', '3').Replace('\u06F4', '4').Replace('\u06F5', '5')
                .Replace('\u06F6', '6').Replace('\u06F7', '7').Replace('\u06F8', '8').Replace('\u06F9', '9');
            return arabicToWestern;
        }

        // Pattern 4: ID mentioned with number (e.g., "my ID is 1234567890")
        var pattern4 = @"(?:id|identity|national\s+id|هوية|رقم\s+الهوية)[\s:]*(\d{10})";
        var match4 = Regex.Match(message, pattern4, RegexOptions.IgnoreCase);
        if (match4.Success)
        {
            return match4.Groups[1].Value;
        }

        return null;
    }

    // Keep old method for backward compatibility but redirect to ID extraction
    public string? ExtractOrderNumber(string message)
    {
        return ExtractIdNumber(message);
    }

    public OrderStatus? GetOrderStatus(string idNumber)
    {
        if (string.IsNullOrWhiteSpace(idNumber))
            return null;

        // Normalize ID number (remove spaces, dashes, etc.)
        var normalizedId = idNumber.Trim().Replace(" ", "").Replace("-", "").Replace(".", "");

        // Try exact match first
        if (_orderStatuses.TryGetValue(normalizedId, out var status))
        {
            return status;
        }

        // Try partial match (in case user entered ID with formatting)
        foreach (var kvp in _orderStatuses)
        {
            var cleanKey = kvp.Key.Replace(" ", "").Replace("-", "").Replace(".", "");
            if (cleanKey == normalizedId || normalizedId.Contains(cleanKey) || cleanKey.Contains(normalizedId))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    public string GetStatusResponse(string idNumber, OrderStatus? status)
    {
        if (status == null)
        {
            return $"Sorry, I couldn't find any applications for ID number '{idNumber}' in the system. " +
                   "Please double-check your ID number and try again. " +
                   "Make sure you're entering your 10-digit Saudi National ID number. " +
                   "If you don't have any active applications, that's why nothing was found.";
        }

        var statusMessage = status.Status switch
        {
            "Submitted" => "We got your request and it's in the queue now.",
            "Under Review" => "Your application's being looked at by our team right now.",
            "Approved" => "Great news! Your request was approved. You might need to do a few more things like pay fees or pick up documents.",
            "Rejected" => $"Unfortunately, your request was rejected. {status.Notes ?? "Check your Absher account for more details on why."}",
            "Completed" => "All done! Your request is complete and ready for pickup or delivery.",
            "Pending" => "Your request is waiting for some info or documents from you. Check your Absher account to see what's needed.",
            _ => $"Your request status is: {status.Status}"
        };

        var response = $"I found an application for ID {idNumber}. It's currently {status.Status.ToLower()}.\n\n" +
                      $"{statusMessage}";

        if (!string.IsNullOrWhiteSpace(status.SubmittedDate))
        {
            response += $"\n\nYou submitted it on {status.SubmittedDate}.";
        }

        if (!string.IsNullOrWhiteSpace(status.LastUpdated))
        {
            response += $" Last update was {status.LastUpdated}.";
        }

        if (!string.IsNullOrWhiteSpace(status.Notes) && status.Status != "Rejected")
        {
            response += $"\n\n{status.Notes}";
        }

        response += "\n\nYou can also check this anytime by logging into Absher and going to 'My Services'.";

        return response;
    }

    private Dictionary<string, OrderStatus> InitializeMockOrders()
    {
        // Mock data using Saudi National ID numbers (10 digits)
        // In production, this would come from a database or API
        return new Dictionary<string, OrderStatus>
        {
            {
                "1234567890",
                new OrderStatus
                {
                    OrderNumber = "1234567890",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your ID renewal application is being processed. Expected completion: 2-3 business days."
                }
            },
            {
                "9876543210",
                new OrderStatus
                {
                    OrderNumber = "9876543210",
                    Status = "Approved",
                    SubmittedDate = "2024-01-10",
                    LastUpdated = "2024-01-16",
                    Notes = "Your passport application has been approved. Please schedule an appointment for biometrics."
                }
            },
            {
                "1122334455",
                new OrderStatus
                {
                    OrderNumber = "1122334455",
                    Status = "Completed",
                    SubmittedDate = "2024-01-05",
                    LastUpdated = "2024-01-12",
                    Notes = "Your driving license renewal is complete. Your new license is ready for pickup."
                }
            },
            {
                "5566778899",
                new OrderStatus
                {
                    OrderNumber = "5566778899",
                    Status = "Pending",
                    SubmittedDate = "2024-01-20",
                    LastUpdated = "2024-01-21",
                    Notes = "Additional documents are required. Please upload the requested documents in your Absher account."
                }
            },
            {
                "2233445566",
                new OrderStatus
                {
                    OrderNumber = "2233445566",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your work permit application is being reviewed."
                }
            }
        };
    }
}

public class OrderStatus
{
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? SubmittedDate { get; set; }
    public string? LastUpdated { get; set; }
    public string? Notes { get; set; }
}

