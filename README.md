# Absher AI Assistant

An intelligent chatbot assistant designed to help users navigate and understand Absher (the Saudi Arabian government portal) services. Built with modern web technologies and AI capabilities.

##  What This Assistant Does (MVP)

The Absher AI Assistant is a conversational chatbot that provides instant help for Absher services through three main categories:

### 1. **Navigation Guidance** 
Helps users find and reach the correct section or service inside Absher quickly and easily.
- Guides users to specific service categories (Civil Affairs, Traffic Services, Labor Services)
- Explains how to navigate the Absher portal
- Provides directions on locating services in the menu structure
- Supports both Arabic and English terms

### 2. **Service Explanation** 
Provides clear, simple explanations of how specific Absher services work and what steps are required.
- Explains step-by-step procedures for services like:
  - National ID renewal
  - Passport applications
  - Work permit processing
  - Driving license renewal
- Describes required documents and prerequisites
- Provides guidance on service processes in plain, conversational language

### 3. **Status Inquiries** 
Allows users to ask about the status of requests or applications and receive instant, AI-generated updates.
- **Id Number**: Automatically extracts Application Status
- Provides real-time status updates including:
  - Submitted
  - Under Review
  - Approved
  - Rejected
  - Completed
  - Pending
- Shows submission dates, last update times, and relevant notes
- Currently uses mock data for demonstration (ready for real-time API integration)

##  Technologies Used to Build This Agent

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







