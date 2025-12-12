# Absher AI Assistant

An intelligent chatbot assistant designed to help users navigate and understand Absher (the Saudi Arabian government portal) services. Built with modern web technologies and AI capabilities.

## 🎯 What This Assistant Does (MVP)

The Absher AI Assistant is a conversational chatbot that provides instant help for Absher services through three main categories:

### 1. **Navigation Guidance** 🗺️
Helps users find and reach the correct section or service inside Absher quickly and easily.
- Guides users to specific service categories (Civil Affairs, Traffic Services, Labor Services)
- Explains how to navigate the Absher portal
- Provides directions on locating services in the menu structure
- Supports both Arabic and English terms

### 2. **Service Explanation** 💼
Provides clear, simple explanations of how specific Absher services work and what steps are required.
- Explains step-by-step procedures for services like:
  - National ID renewal
  - Passport applications
  - Work permit processing
  - Driving license renewal
- Describes required documents and prerequisites
- Provides guidance on service processes in plain, conversational language

### 3. **Status Inquiries** 📊
Allows users to ask about the status of requests or applications and receive instant, AI-generated updates.
- **Order Number Tracking**: Automatically extracts order numbers (REQ-XXXXXX, APP-XXXXXX) from user queries
- Provides real-time status updates including:
  - Submitted
  - Under Review
  - Approved
  - Rejected
  - Completed
  - Pending
- Shows submission dates, last update times, and relevant notes
- Currently uses mock data for demonstration (ready for real-time API integration)

### Additional Features
- **Website Content Loading**: Users can load Absher website content and ask questions about it
- **Voice Input**: Press-and-hold voice recognition for hands-free interaction
- **Mobile Responsive**: Fully optimized for desktop, tablet, and mobile devices
- **Natural Language**: Responses are written in a human-like, conversational style
- **Multi-language Support**: Handles both English and Arabic terms

## 🚀 Future Capabilities with Real-Time Data

### Phase 1: Real-Time Integration
- **Live Order Status**: Connect to Absher API to fetch real-time order/request status
- **Live Service Availability**: Check which services are currently available or under maintenance
- **Real-Time Processing Times**: Get accurate, up-to-date processing times based on current system load
- **Appointment Scheduling**: Integration with Absher's appointment system to book and manage appointments

### Phase 2: Advanced Features
- **User Authentication**: Secure login integration with Absher accounts
- **Personalized Dashboard**: Show user-specific information (family members, workers, documents)
- **Document Management**: View and download digital documents (Iqama, licenses, certificates)
- **Payment Integration**: Process payments for services directly through the chatbot
- **Notification System**: Real-time SMS/email notifications for status updates
- **Multi-language Full Support**: Complete Arabic language support with RTL layout

### Phase 3: Intelligent Features
- **Predictive Assistance**: AI predicts what users might need based on their profile
- **Smart Recommendations**: Suggest relevant services based on user history
- **Document Upload**: Allow users to upload required documents through the chat
- **Voice Commands**: Full voice interaction for all features
- **Chat History**: Save conversation history for logged-in users
- **Analytics Dashboard**: Track common questions and improve responses

### Phase 4: Enterprise Features
- **Admin Panel**: Manage knowledge base, monitor conversations, view analytics
- **Custom Integrations**: API for third-party integrations
- **Advanced Analytics**: Detailed insights into user behavior and service usage
- **A/B Testing**: Test different response strategies
- **Multi-tenant Support**: Support for multiple government portals

## 🛠️ Technologies Used to Build This Agent

### Backend Framework
- **.NET 8.0**: Modern, high-performance framework
- **Blazor Server**: Real-time, interactive web UI with server-side rendering
- **C#**: Primary programming language

### AI & Machine Learning
- **OpenAI GPT-3.5-turbo**: Large language model for intelligent, context-aware responses
- **Custom Prompt Engineering**: Optimized prompts for natural, human-like conversations
- **Conversation Context**: Maintains last 10 messages for contextual understanding

### Services Architecture
- **ChatService**: Main orchestration service for message processing
- **OpenAIService**: Handles OpenAI API integration with fallback mechanisms
- **KnowledgeBaseService**: JSON-based knowledge base for predefined responses
- **DocumentService**: Extracts and processes content from documents and websites
- **OrderStatusService**: Order number extraction and status lookup system

### Frontend Technologies
- **Blazor Components**: Razor components for UI
- **CSS3**: Custom styling with responsive design
- **JavaScript Interop**: Web Speech API for voice input
- **Bootstrap**: UI framework for responsive layouts

### Data Storage
- **JSON Knowledge Base**: Lightweight, easy-to-maintain knowledge base
- **In-Memory Services**: Fast, efficient data handling
- **Static File Storage**: Document and image storage

### APIs & Integrations
- **OpenAI REST API**: GPT-3.5-turbo chat completions
- **Web Speech API**: Browser-native voice recognition
- **HTTP Client**: For fetching website content

### Development Tools
- **Visual Studio / VS Code**: Development environment
- **Git**: Version control
- **NuGet**: Package management

## 📁 Project Structure

```
AIChatbot/
├── Components/
│   ├── Chatbot/
│   │   └── ChatbotWidget.razor      # Main chatbot UI component
│   └── Layout/
│       └── MainLayout.razor         # Application layout
├── Services/
│   ├── ChatService.cs               # Main chat orchestration
│   ├── OpenAIService.cs             # OpenAI integration
│   ├── KnowledgeBaseService.cs      # Knowledge base handler
│   ├── DocumentService.cs           # Document/website processing
│   └── OrderStatusService.cs        # Order status tracking
├── wwwroot/
│   ├── knowledge-base.json          # Knowledge base data
│   ├── img/
│   │   └── cover.jpeg              # Background image
│   └── js/
│       └── speech-recognition.js    # Voice input handler
└── Program.cs                       # Application entry point
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- OpenAI API key (optional - falls back to knowledge base if not configured)

### Configuration
1. Set your OpenAI API key in `appsettings.json` (optional)
2. Customize knowledge base in `wwwroot/knowledge-base.json`
3. Run the application: `dotnet run`

### Usage
- Click the green chat button in the bottom-right corner
- Select a category (Navigation, Services, or Status)
- Ask questions naturally - the AI will understand and respond
- Use voice input by holding the microphone button

## 📝 Current Limitations (MVP)

- Order status uses mock data (ready for API integration)
- Knowledge base is static JSON (can be enhanced with database)
- No user authentication (all interactions are anonymous)
- Limited to English with some Arabic terms
- No conversation history persistence

## 🔮 Roadmap

See "Future Capabilities" section above for planned enhancements. The architecture is designed to easily integrate:
- Real-time APIs
- Database storage
- User authentication
- Advanced AI features
- Multi-language support

## 📄 License

This project is proprietary software developed for Absher services.

## 🤝 Support

For issues or questions, please contact the development team.

---

**Built with ❤️ for Absher users**

