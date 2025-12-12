# Handover Documentation - JMM Innovations AI Chatbot Widget

## Project Summary

This document provides instructions for maintaining, updating, and redeploying the AI Chatbot Widget MVP delivered for JMM Innovations.

## Project Structure

```
├── widget/
│   ├── chatbot-widget.js      # Main widget JavaScript (self-contained)
│   └── knowledge-base.json    # Knowledge base with all responses
├── demo.html                   # Demo webpage showcasing the widget
├── README.md                   # User documentation
└── HANDOVER.md                 # This file
```

## How to Edit/Update the Knowledge Base

### Location
The knowledge base is located at: `widget/knowledge-base.json`

### Structure
The knowledge base has three main categories:
1. **navigation** - For location and directions questions
2. **services** - For service-related inquiries
3. **status** - For status and tracking questions

### Editing Process

1. **Open the file:** `widget/knowledge-base.json`

2. **Add/Edit Keywords:**
   - Keywords are words or phrases that trigger responses
   - Add keywords to the `keywords` array
   - Example: `["where", "location", "find", "directions"]`

3. **Add/Edit Responses:**
   - Responses are the answers the chatbot will give
   - Add multiple responses for variety
   - The widget randomly selects one response when keywords match
   - Example: `["Response 1", "Response 2", "Response 3"]`

4. **Example Edit:**
   ```json
   {
     "navigation": [
       {
         "keywords": ["where", "location", "find", "directions"],
         "responses": [
           "Your new response here",
           "Another response option"
         ]
       }
     ]
   }
   ```

5. **Save the file** - Changes take effect immediately on page reload

### Best Practices

- **Multiple responses:** Add 3-5 responses per keyword set for variety
- **Keyword coverage:** Include common variations and synonyms
- **Clear responses:** Keep responses concise and helpful
- **Test after changes:** Always test the widget after updating the knowledge base

## How to Redeploy

### Option 1: Static Hosting (Recommended)

#### Netlify
1. Go to [netlify.com](https://netlify.com)
2. Drag and drop the project folder, OR
3. Connect via Git repository
4. Site will auto-deploy on changes

#### Vercel
1. Install Vercel CLI: `npm i -g vercel`
2. Navigate to project directory
3. Run: `vercel`
4. Follow prompts to deploy

#### GitHub Pages
1. Push code to GitHub repository
2. Go to Settings > Pages
3. Select branch and folder
4. Save - site will be available at `username.github.io/repo-name`

### Option 2: Traditional Web Server

1. **Upload files** to your web server via FTP/SFTP
2. **Maintain directory structure:**
   ```
   /public_html/
     ├── demo.html
     └── widget/
         ├── chatbot-widget.js
         └── knowledge-base.json
   ```
3. **Ensure permissions:** Files should be readable (644)
4. **Test:** Visit your domain/demo.html

### Option 3: CDN/Cloud Storage

#### AWS S3 + CloudFront
1. Upload files to S3 bucket
2. Enable static website hosting
3. Configure CloudFront distribution
4. Update DNS if needed

#### Google Cloud Storage
1. Create bucket
2. Upload files
3. Make bucket public
4. Enable static website hosting

## Configuration Options

When embedding the widget, you can configure:

```javascript
window.JMMChatbot = new ChatbotWidget({
    position: 'bottom-right',      // Widget position
    primaryColor: '#007bff',        // Brand color
    knowledgeBase: 'widget/knowledge-base.json'  // Knowledge base path
});
```

### Position Options
- `'bottom-right'` (default)
- `'bottom-left'`
- `'top-right'`
- `'top-left'`

### Color Customization
Change `primaryColor` to match your brand:
- Hex: `'#007bff'`
- RGB: Not directly supported, use hex

## Testing Checklist

After deployment, test:

- [ ] Widget appears on page
- [ ] Chat opens/closes correctly
- [ ] Text input works
- [ ] Send button works
- [ ] Voice button appears (if supported)
- [ ] Voice input works (test in Chrome/Safari)
- [ ] Knowledge base responses appear correctly
- [ ] All three categories work (navigation, services, status)
- [ ] Mobile responsiveness works
- [ ] No console errors

## Troubleshooting

### Widget Not Loading
- Check browser console for errors
- Verify `chatbot-widget.js` path is correct
- Ensure JavaScript is enabled

### Knowledge Base Not Loading
- Check file path in initialization code
- Verify JSON is valid (use JSON validator)
- Check browser console for CORS errors
- Ensure file is accessible (not blocked by server)

### Voice Not Working
- Requires Chrome or Safari
- Check microphone permissions
- HTTPS may be required
- Check browser console for errors

### Styling Issues
- Widget uses inline styles, should work everywhere
- Check for CSS conflicts with parent page
- Verify z-index (widget uses 10000)

## Maintenance Notes

### Regular Updates
- **Knowledge Base:** Update as business needs change
- **Responses:** Refresh responses periodically for relevance
- **Keywords:** Add new keywords based on user questions

### Performance
- Widget is lightweight (~15KB JavaScript)
- No external dependencies
- Fast loading and rendering

### Browser Support
- Modern browsers (Chrome, Safari, Edge, Firefox)
- Mobile browsers supported
- Voice requires Chrome/Safari

## Contact & Support

For technical questions or issues:
- Email: partner@jmminnovations.com
- Check README.md for detailed documentation

## Version History

- **v1.0.0** (Dec 2024) - Initial MVP delivery
  - Text and voice input
  - Knowledge base system
  - Three response categories
  - Self-contained widget

---

**Document Version:** 1.0  
**Last Updated:** December 2024  
**Prepared for:** JMM Innovations



