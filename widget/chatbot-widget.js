/**
 * JMM Innovations AI Chatbot Widget
 * Self-contained floating chat widget with text and voice input
 */

class ChatbotWidget {
    constructor(config = {}) {
        this.isOpen = false;
        this.isListening = false;
        this.recognition = null;
        this.messages = [];
        this.config = {
            position: config.position || 'bottom-right',
            primaryColor: config.primaryColor || '#007bff',
            knowledgeBase: config.knowledgeBase || null
        };
        
        this.init();
    }

    init() {
        this.createWidgetHTML();
        this.attachEventListeners();
        this.loadKnowledgeBase();
        this.initSpeechRecognition();
    }

    createWidgetHTML() {
        const widgetHTML = `
            <div id="jmm-chatbot-widget" class="jmm-widget-container">
                <div class="jmm-widget-button" id="jmm-widget-toggle">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
                    </svg>
                </div>
                <div class="jmm-widget-chat" id="jmm-widget-chat" style="display: none;">
                    <div class="jmm-widget-header">
                        <h3>AI Assistant</h3>
                        <button class="jmm-widget-close" id="jmm-widget-close">×</button>
                    </div>
                    <div class="jmm-widget-messages" id="jmm-widget-messages">
                        <div class="jmm-message jmm-message-bot">
                            <div class="jmm-message-content">
                                Hello! I'm your AI assistant. How can I help you today? You can ask me about navigation, services, or check status.
                            </div>
                        </div>
                    </div>
                    <div class="jmm-widget-input-container">
                        <div class="jmm-input-wrapper">
                            <input type="text" 
                                   id="jmm-widget-text-input" 
                                   class="jmm-widget-input" 
                                   placeholder="Type your message..."
                                   autocomplete="off">
                            <button class="jmm-voice-button" 
                                    id="jmm-voice-button" 
                                    title="Press and hold to speak">
                                <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                                    <path d="M12 14c1.66 0 3-1.34 3-3V5c0-1.66-1.34-3-3-3S9 3.34 9 5v6c0 1.66 1.34 3 3 3z"></path>
                                    <path d="M17 11c0 2.76-2.24 5-5 5s-5-2.24-5-5H5c0 3.53 2.61 6.43 6 6.92V21h2v-3.08c3.39-.49 6-3.39 6-6.92h-2z"></path>
                                </svg>
                            </button>
                        </div>
                        <button class="jmm-send-button" id="jmm-send-button">
                            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <line x1="22" y1="2" x2="11" y2="13"></line>
                                <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
                            </svg>
                        </button>
                    </div>
                    <div class="jmm-listening-indicator" id="jmm-listening-indicator" style="display: none;">
                        <div class="jmm-pulse"></div>
                        <span>Listening...</span>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', widgetHTML);
        this.applyStyles();
    }

    applyStyles() {
        const style = document.createElement('style');
        style.textContent = `
            .jmm-widget-container {
                position: fixed;
                ${this.config.position === 'bottom-right' ? 'bottom: 20px; right: 20px;' : ''}
                ${this.config.position === 'bottom-left' ? 'bottom: 20px; left: 20px;' : ''}
                ${this.config.position === 'top-right' ? 'top: 20px; right: 20px;' : ''}
                ${this.config.position === 'top-left' ? 'top: 20px; left: 20px;' : ''}
                z-index: 10000;
                font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            }

            .jmm-widget-button {
                width: 60px;
                height: 60px;
                border-radius: 50%;
                background: ${this.config.primaryColor};
                color: white;
                display: flex;
                align-items: center;
                justify-content: center;
                cursor: pointer;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                transition: transform 0.2s, box-shadow 0.2s;
            }

            .jmm-widget-button:hover {
                transform: scale(1.1);
                box-shadow: 0 6px 16px rgba(0, 0, 0, 0.2);
            }

            .jmm-widget-chat {
                position: absolute;
                bottom: 80px;
                right: 0;
                width: 380px;
                height: 600px;
                background: white;
                border-radius: 16px;
                box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15);
                display: flex;
                flex-direction: column;
                overflow: hidden;
            }

            .jmm-widget-header {
                background: ${this.config.primaryColor};
                color: white;
                padding: 16px 20px;
                display: flex;
                justify-content: space-between;
                align-items: center;
            }

            .jmm-widget-header h3 {
                margin: 0;
                font-size: 18px;
                font-weight: 600;
            }

            .jmm-widget-close {
                background: none;
                border: none;
                color: white;
                font-size: 28px;
                cursor: pointer;
                padding: 0;
                width: 32px;
                height: 32px;
                display: flex;
                align-items: center;
                justify-content: center;
                border-radius: 50%;
                transition: background 0.2s;
            }

            .jmm-widget-close:hover {
                background: rgba(255, 255, 255, 0.2);
            }

            .jmm-widget-messages {
                flex: 1;
                overflow-y: auto;
                padding: 20px;
                background: #f8f9fa;
            }

            .jmm-message {
                margin-bottom: 16px;
                display: flex;
                animation: fadeIn 0.3s;
            }

            @keyframes fadeIn {
                from { opacity: 0; transform: translateY(10px); }
                to { opacity: 1; transform: translateY(0); }
            }

            .jmm-message-user {
                justify-content: flex-end;
            }

            .jmm-message-bot {
                justify-content: flex-start;
            }

            .jmm-message-content {
                max-width: 75%;
                padding: 12px 16px;
                border-radius: 18px;
                word-wrap: break-word;
                line-height: 1.4;
            }

            .jmm-message-user .jmm-message-content {
                background: ${this.config.primaryColor};
                color: white;
                border-bottom-right-radius: 4px;
            }

            .jmm-message-bot .jmm-message-content {
                background: white;
                color: #333;
                border-bottom-left-radius: 4px;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            }

            .jmm-widget-input-container {
                padding: 16px;
                background: white;
                border-top: 1px solid #e9ecef;
                display: flex;
                gap: 8px;
                align-items: flex-end;
            }

            .jmm-input-wrapper {
                flex: 1;
                display: flex;
                align-items: center;
                border: 1px solid #dee2e6;
                border-radius: 24px;
                padding: 8px 12px;
                background: #f8f9fa;
            }

            .jmm-widget-input {
                flex: 1;
                border: none;
                outline: none;
                background: transparent;
                font-size: 14px;
                padding: 4px 8px;
            }

            .jmm-voice-button {
                background: none;
                border: none;
                color: ${this.config.primaryColor};
                cursor: pointer;
                padding: 4px;
                display: flex;
                align-items: center;
                justify-content: center;
                border-radius: 50%;
                transition: background 0.2s;
            }

            .jmm-voice-button:hover {
                background: rgba(0, 123, 255, 0.1);
            }

            .jmm-voice-button.active {
                background: rgba(220, 53, 69, 0.1);
                color: #dc3545;
            }

            .jmm-send-button {
                width: 40px;
                height: 40px;
                border-radius: 50%;
                background: ${this.config.primaryColor};
                color: white;
                border: none;
                cursor: pointer;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: background 0.2s, transform 0.2s;
            }

            .jmm-send-button:hover {
                background: #0056b3;
                transform: scale(1.05);
            }

            .jmm-listening-indicator {
                padding: 12px 16px;
                background: #fff3cd;
                border-top: 1px solid #ffc107;
                display: flex;
                align-items: center;
                gap: 12px;
                color: #856404;
                font-size: 14px;
            }

            .jmm-pulse {
                width: 12px;
                height: 12px;
                background: #dc3545;
                border-radius: 50%;
                animation: pulse 1.5s infinite;
            }

            @keyframes pulse {
                0%, 100% { opacity: 1; transform: scale(1); }
                50% { opacity: 0.5; transform: scale(1.2); }
            }

            @media (max-width: 480px) {
                .jmm-widget-chat {
                    width: calc(100vw - 40px);
                    height: calc(100vh - 100px);
                    bottom: 80px;
                    right: 20px;
                    left: 20px;
                }
            }
        `;
        document.head.appendChild(style);
    }

    attachEventListeners() {
        const toggleBtn = document.getElementById('jmm-widget-toggle');
        const closeBtn = document.getElementById('jmm-widget-close');
        const sendBtn = document.getElementById('jmm-send-button');
        const textInput = document.getElementById('jmm-widget-text-input');
        const voiceBtn = document.getElementById('jmm-voice-button');

        toggleBtn.addEventListener('click', () => this.toggleChat());
        closeBtn.addEventListener('click', () => this.toggleChat());
        sendBtn.addEventListener('click', () => this.sendMessage());
        textInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') this.sendMessage();
        });

        // Voice button - press and hold
        voiceBtn.addEventListener('mousedown', () => this.startListening());
        voiceBtn.addEventListener('mouseup', () => this.stopListening());
        voiceBtn.addEventListener('mouseleave', () => this.stopListening());
        
        // Touch events for mobile
        voiceBtn.addEventListener('touchstart', (e) => {
            e.preventDefault();
            this.startListening();
        });
        voiceBtn.addEventListener('touchend', (e) => {
            e.preventDefault();
            this.stopListening();
        });
    }

    initSpeechRecognition() {
        if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
            const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
            this.recognition = new SpeechRecognition();
            this.recognition.continuous = false;
            this.recognition.interimResults = false;
            this.recognition.lang = 'en-US';

            this.recognition.onresult = (event) => {
                const transcript = event.results[0][0].transcript;
                this.addUserMessage(transcript);
                this.processMessage(transcript);
            };

            this.recognition.onerror = (event) => {
                console.error('Speech recognition error:', event.error);
                this.stopListening();
                if (event.error === 'no-speech') {
                    this.addBotMessage("I didn't catch that. Please try again.");
                }
            };

            this.recognition.onend = () => {
                this.stopListening();
            };
        } else {
            const voiceBtn = document.getElementById('jmm-voice-button');
            voiceBtn.style.display = 'none';
            console.warn('Speech recognition not supported in this browser');
        }
    }

    startListening() {
        if (!this.recognition) return;
        
        if (!this.isOpen) {
            this.toggleChat();
        }

        this.isListening = true;
        const voiceBtn = document.getElementById('jmm-voice-button');
        const indicator = document.getElementById('jmm-listening-indicator');
        
        voiceBtn.classList.add('active');
        indicator.style.display = 'flex';
        
        try {
            this.recognition.start();
        } catch (e) {
            console.error('Error starting recognition:', e);
            this.stopListening();
        }
    }

    stopListening() {
        if (!this.recognition) return;
        
        this.isListening = false;
        const voiceBtn = document.getElementById('jmm-voice-button');
        const indicator = document.getElementById('jmm-listening-indicator');
        
        voiceBtn.classList.remove('active');
        indicator.style.display = 'none';
        
        try {
            this.recognition.stop();
        } catch (e) {
            // Ignore errors when stopping
        }
    }

    toggleChat() {
        this.isOpen = !this.isOpen;
        const chat = document.getElementById('jmm-widget-chat');
        const toggleBtn = document.getElementById('jmm-widget-toggle');
        
        if (this.isOpen) {
            chat.style.display = 'flex';
            toggleBtn.style.display = 'none';
            document.getElementById('jmm-widget-text-input').focus();
        } else {
            chat.style.display = 'none';
            toggleBtn.style.display = 'flex';
        }
    }

    sendMessage() {
        const textInput = document.getElementById('jmm-widget-text-input');
        const message = textInput.value.trim();
        
        if (message) {
            this.addUserMessage(message);
            this.processMessage(message);
            textInput.value = '';
        }
    }

    addUserMessage(text) {
        this.messages.push({ type: 'user', text });
        this.renderMessage('user', text);
    }

    addBotMessage(text) {
        this.messages.push({ type: 'bot', text });
        this.renderMessage('bot', text);
    }

    renderMessage(type, text) {
        const messagesContainer = document.getElementById('jmm-widget-messages');
        const messageDiv = document.createElement('div');
        messageDiv.className = `jmm-message jmm-message-${type}`;
        messageDiv.innerHTML = `<div class="jmm-message-content">${this.escapeHtml(text)}</div>`;
        
        messagesContainer.appendChild(messageDiv);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    async loadKnowledgeBase() {
        if (this.config.knowledgeBase) {
            try {
                const response = await fetch(this.config.knowledgeBase);
                this.knowledgeBase = await response.json();
            } catch (error) {
                console.error('Error loading knowledge base:', error);
                this.knowledgeBase = this.getDefaultKnowledgeBase();
            }
        } else {
            this.knowledgeBase = this.getDefaultKnowledgeBase();
        }
    }

    getDefaultKnowledgeBase() {
        return {
            navigation: [
                {
                    keywords: ['where', 'location', 'find', 'directions', 'how to get', 'navigate'],
                    responses: [
                        "You can find us at 123 Main Street. We're open Monday to Friday, 9 AM to 5 PM.",
                        "Our office is located in the downtown area. You can reach us by taking the metro to Central Station.",
                        "For directions, please visit our website or call us at +1-234-567-8900."
                    ]
                }
            ],
            services: [
                {
                    keywords: ['service', 'what do you', 'offer', 'provide', 'help with', 'can you'],
                    responses: [
                        "We offer a wide range of services including consulting, development, and support. How can I assist you today?",
                        "Our services include business consulting, technology solutions, and customer support. What specific area interests you?",
                        "We provide comprehensive solutions for your business needs. Would you like to know more about a specific service?"
                    ]
                }
            ],
            status: [
                {
                    keywords: ['status', 'check', 'order', 'request', 'track', 'update', 'when'],
                    responses: [
                        "Your request is currently being processed. Expected completion: within 2-3 business days.",
                        "I've checked your status - everything is on track. You'll receive an update via email shortly.",
                        "Status: In Progress. Our team is working on your request and will notify you upon completion."
                    ]
                }
            ]
        };
    }

    processMessage(message) {
        const lowerMessage = message.toLowerCase();
        let response = null;
        let category = null;

        // Check navigation
        for (const item of this.knowledgeBase.navigation) {
            if (item.keywords.some(keyword => lowerMessage.includes(keyword))) {
                category = 'navigation';
                response = item.responses[Math.floor(Math.random() * item.responses.length)];
                break;
            }
        }

        // Check services
        if (!response) {
            for (const item of this.knowledgeBase.services) {
                if (item.keywords.some(keyword => lowerMessage.includes(keyword))) {
                    category = 'services';
                    response = item.responses[Math.floor(Math.random() * item.responses.length)];
                    break;
                }
            }
        }

        // Check status
        if (!response) {
            for (const item of this.knowledgeBase.status) {
                if (item.keywords.some(keyword => lowerMessage.includes(keyword))) {
                    category = 'status';
                    response = item.responses[Math.floor(Math.random() * item.responses.length)];
                    break;
                }
            }
        }

        // Default response
        if (!response) {
            response = "I understand you're asking about: \"" + message + "\". Could you please rephrase your question? I can help with navigation, services, or status inquiries.";
        }

        // Simulate typing delay
        setTimeout(() => {
            this.addBotMessage(response);
        }, 500);
    }
}

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.JMMChatbot = new ChatbotWidget();
    });
} else {
    window.JMMChatbot = new ChatbotWidget();
}


