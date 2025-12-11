// Speech Recognition JavaScript Interop for Blazor
let recognition = null;
let dotNetRef = null;
let isListening = false;

export function initialize(dotNetReference) {
    dotNetRef = dotNetReference;
    
    if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
        try {
            const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
            recognition = new SpeechRecognition();
            recognition.continuous = false;
            recognition.interimResults = false;
            recognition.lang = 'en-US';
            recognition.maxAlternatives = 1;

            recognition.onstart = function() {
                console.log('Speech recognition started');
                isListening = true;
            };

            recognition.onresult = function(event) {
                if (event.results && event.results.length > 0 && event.results[0].length > 0) {
                    const transcript = event.results[0][0].transcript.trim();
                    console.log('Speech recognized:', transcript);
                    if (dotNetRef && transcript) {
                        dotNetRef.invokeMethodAsync('OnSpeechResult', transcript).catch(err => {
                            console.error('Error calling OnSpeechResult:', err);
                        });
                    }
                }
            };

            recognition.onerror = function(event) {
                console.error('Speech recognition error:', event.error);
                isListening = false;
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnSpeechError', event.error).catch(err => {
                        console.error('Error calling OnSpeechError:', err);
                    });
                }
            };

            recognition.onend = function() {
                console.log('Speech recognition ended');
                isListening = false;
                // Notify Blazor that listening has stopped
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnSpeechEnd').catch(err => {
                        console.error('Error calling OnSpeechEnd:', err);
                    });
                }
            };
        } catch (error) {
            console.error('Error initializing speech recognition:', error);
        }
    } else {
        console.warn('Speech recognition not supported in this browser');
    }
}

export function startListening() {
    if (!recognition) {
        console.warn('Speech recognition not available');
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSpeechError', 'not-available').catch(err => {
                console.error('Error calling OnSpeechError:', err);
            });
        }
        return;
    }

    if (isListening) {
        console.log('Already listening, ignoring start request');
        return;
    }

    try {
        // Stop any existing recognition first
        if (recognition && recognition.state !== 'inactive') {
            recognition.stop();
        }
        
        // Small delay to ensure previous recognition is fully stopped
        setTimeout(() => {
            try {
                recognition.start();
            } catch (error) {
                console.error('Error starting recognition:', error);
                isListening = false;
                if (dotNetRef) {
                    dotNetRef.invokeMethodAsync('OnSpeechError', error.message || 'start-failed').catch(err => {
                        console.error('Error calling OnSpeechError:', err);
                    });
                }
            }
        }, 100);
    } catch (error) {
        console.error('Error in startListening:', error);
        isListening = false;
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSpeechError', error.message || 'start-failed').catch(err => {
                console.error('Error calling OnSpeechError:', err);
            });
        }
    }
}

export function stopListening() {
    if (!recognition) {
        return;
    }

    try {
        if (recognition.state === 'listening' || recognition.state === 'starting') {
            recognition.stop();
        }
        isListening = false;
    } catch (error) {
        console.error('Error stopping recognition:', error);
        isListening = false;
    }
}

export function dispose() {
    if (recognition && isListening) {
        try {
            recognition.stop();
        } catch (error) {
            // Ignore
        }
    }
    recognition = null;
    dotNetRef = null;
}

// Helper function to scroll messages container
window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

