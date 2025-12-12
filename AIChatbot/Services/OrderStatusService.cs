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

    public string? ExtractOrderNumber(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return null;

        // Common order number patterns in Absher
        // Pattern 1: REQ-XXXXXX format
        var pattern1 = @"REQ-?\s*(\d{6,})";
        var match1 = Regex.Match(message, pattern1, RegexOptions.IgnoreCase);
        if (match1.Success)
        {
            return $"REQ-{match1.Groups[1].Value}";
        }

        // Pattern 2: APP-XXXXXX format
        var pattern2 = @"APP-?\s*(\d{6,})";
        var match2 = Regex.Match(message, pattern2, RegexOptions.IgnoreCase);
        if (match2.Success)
        {
            return $"APP-{match2.Groups[1].Value}";
        }

        // Pattern 3: Just numbers (6-12 digits)
        var pattern3 = @"\b(\d{6,12})\b";
        var match3 = Regex.Match(message, pattern3);
        if (match3.Success)
        {
            return match3.Groups[1].Value;
        }

        // Pattern 4: Arabic numerals or mixed
        var pattern4 = @"[\u0660-\u0669\u06F0-\u06F9\d]{6,}";
        var match4 = Regex.Match(message, pattern4);
        if (match4.Success)
        {
            return match4.Value;
        }

        return null;
    }

    public OrderStatus? GetOrderStatus(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            return null;

        // Normalize order number (remove spaces, convert to uppercase)
        var normalizedOrder = orderNumber.Trim().ToUpper().Replace(" ", "");

        // Try exact match first
        if (_orderStatuses.TryGetValue(normalizedOrder, out var status))
        {
            return status;
        }

        // Try partial match (in case user entered just the number part)
        foreach (var kvp in _orderStatuses)
        {
            if (kvp.Key.Contains(normalizedOrder) || normalizedOrder.Contains(kvp.Key.Replace("REQ-", "").Replace("APP-", "")))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    public string GetStatusResponse(string orderNumber, OrderStatus? status)
    {
        if (status == null)
        {
            return $"I couldn't find an order with number '{orderNumber}' in the Absher system. " +
                   "Please verify the order number and try again. " +
                   "Order numbers are typically in formats like REQ-123456 or APP-123456. " +
                   "You can find your order number in the confirmation email or SMS you received when you submitted your request.";
        }

        var statusMessage = status.Status switch
        {
            "Submitted" => "Your request has been received and is in queue for processing.",
            "Under Review" => "Your application is currently being reviewed by our team.",
            "Approved" => "Your request has been approved! You may need to complete additional steps like payment or document pickup.",
            "Rejected" => $"Your request has been rejected. Reason: {status.Notes ?? "Please check your Absher account for details."}",
            "Completed" => "Your request has been completed and is ready for pickup or delivery.",
            "Pending" => "Your request is pending additional information or documents. Please check your Absher account for required actions.",
            _ => $"Your request status is: {status.Status}"
        };

        var response = $"📋 **Order Number:** {orderNumber}\n\n" +
                      $"📊 **Status:** {status.Status}\n\n" +
                      $"{statusMessage}\n\n";

        if (!string.IsNullOrWhiteSpace(status.SubmittedDate))
        {
            response += $"📅 **Submitted:** {status.SubmittedDate}\n";
        }

        if (!string.IsNullOrWhiteSpace(status.LastUpdated))
        {
            response += $"🔄 **Last Updated:** {status.LastUpdated}\n";
        }

        if (!string.IsNullOrWhiteSpace(status.Notes) && status.Status != "Rejected")
        {
            response += $"\n📝 **Notes:** {status.Notes}";
        }

        response += "\n\nYou can also check your status by logging into your Absher account and going to 'My Services' section.";

        return response;
    }

    private Dictionary<string, OrderStatus> InitializeMockOrders()
    {
        // Mock order data for demonstration
        // In production, this would come from a database or API
        return new Dictionary<string, OrderStatus>
        {
            {
                "REQ-123456",
                new OrderStatus
                {
                    OrderNumber = "REQ-123456",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your ID renewal application is being processed. Expected completion: 2-3 business days."
                }
            },
            {
                "REQ-789012",
                new OrderStatus
                {
                    OrderNumber = "REQ-789012",
                    Status = "Approved",
                    SubmittedDate = "2024-01-10",
                    LastUpdated = "2024-01-16",
                    Notes = "Your passport application has been approved. Please schedule an appointment for biometrics."
                }
            },
            {
                "APP-345678",
                new OrderStatus
                {
                    OrderNumber = "APP-345678",
                    Status = "Completed",
                    SubmittedDate = "2024-01-05",
                    LastUpdated = "2024-01-12",
                    Notes = "Your driving license renewal is complete. Your new license is ready for pickup."
                }
            },
            {
                "REQ-901234",
                new OrderStatus
                {
                    OrderNumber = "REQ-901234",
                    Status = "Pending",
                    SubmittedDate = "2024-01-20",
                    LastUpdated = "2024-01-21",
                    Notes = "Additional documents are required. Please upload the requested documents in your Absher account."
                }
            },
            {
                "123456",
                new OrderStatus
                {
                    OrderNumber = "REQ-123456",
                    Status = "Under Review",
                    SubmittedDate = "2024-01-15",
                    LastUpdated = "2024-01-18",
                    Notes = "Your ID renewal application is being processed."
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

