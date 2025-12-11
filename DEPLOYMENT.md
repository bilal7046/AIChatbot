# Deployment Guide - JMM Innovations AI Chatbot Widget

## Quick Deployment Options

### 1. Netlify (Easiest - Recommended)

**Steps:**
1. Go to [app.netlify.com](https://app.netlify.com)
2. Sign up/Login
3. Click "Add new site" > "Deploy manually"
4. Drag and drop the entire project folder
5. Wait for deployment (usually 30-60 seconds)
6. Your site will be live at: `https://random-name.netlify.app`
7. You can customize the domain name in site settings

**Advantages:**
- Free tier available
- Automatic HTTPS
- Fast CDN
- Easy updates (just drag and drop again)

### 2. Vercel

**Steps:**
1. Install Vercel CLI: `npm i -g vercel` (requires Node.js)
2. Open terminal in project directory
3. Run: `vercel`
4. Follow prompts (press Enter for defaults)
5. Your site will be live at: `https://project-name.vercel.app`

**Advantages:**
- Free tier available
- Automatic HTTPS
- Git integration available
- Fast global CDN

### 3. GitHub Pages

**Steps:**
1. Create a GitHub repository
2. Upload all files to the repository
3. Go to Settings > Pages
4. Under "Source", select branch (usually `main` or `master`)
5. Select folder (usually `/root`)
6. Click Save
7. Your site will be live at: `https://username.github.io/repository-name`

**Advantages:**
- Free
- Version control built-in
- Easy updates via Git push

### 4. Traditional Web Hosting

**Steps:**
1. Connect to your web server via FTP/SFTP (FileZilla, WinSCP, etc.)
2. Navigate to public HTML directory (usually `public_html` or `www`)
3. Upload all files maintaining this structure:
   ```
   public_html/
     ├── demo.html
     └── widget/
         ├── chatbot-widget.js
         └── knowledge-base.json
   ```
4. Visit: `https://yourdomain.com/demo.html`

**Advantages:**
- Full control
- Use existing domain
- No third-party dependencies

## Testing Your Deployment

After deployment, test these:

1. **Basic Functionality:**
   - Open the demo page
   - Widget should appear in bottom-right corner
   - Click to open chat

2. **Text Input:**
   - Type a message
   - Press Enter or click Send
   - Response should appear

3. **Voice Input:**
   - Click and hold microphone button
   - Speak a question
   - Release button
   - Response should appear

4. **Knowledge Base:**
   - Ask: "Where are you located?" (navigation)
   - Ask: "What services do you offer?" (services)
   - Ask: "What's my status?" (status)

5. **Mobile Testing:**
   - Test on mobile device
   - Widget should be responsive
   - Touch interactions should work

## Environment-Specific Notes

### Local Development

For local testing, you need a web server (files won't work with `file://` protocol due to JSON loading):

**Python:**
```bash
python -m http.server 8000
# Then visit: http://localhost:8000/demo.html
```

**Node.js:**
```bash
npx http-server
# Then visit: http://localhost:8080/demo.html
```

**PHP:**
```bash
php -S localhost:8000
# Then visit: http://localhost:8000/demo.html
```

### Production Considerations

1. **HTTPS Required for Voice:**
   - Voice input requires HTTPS in most browsers
   - All recommended hosting options provide HTTPS automatically

2. **CORS (if loading from different domain):**
   - If knowledge base is on different domain, ensure CORS headers
   - Most static hosts handle this automatically

3. **File Permissions:**
   - Ensure files are readable (644 permissions)
   - Directories should be executable (755)

4. **Cache Control:**
   - Consider cache headers for knowledge-base.json
   - Allows easy updates without hard refresh

## Custom Domain Setup

### Netlify
1. Go to Site settings > Domain management
2. Click "Add custom domain"
3. Follow DNS configuration instructions

### Vercel
1. Go to Project settings > Domains
2. Add your domain
3. Update DNS records as instructed

### GitHub Pages
1. Add `CNAME` file with your domain name
2. Update DNS records to point to GitHub Pages
3. Wait for DNS propagation

## Updating After Deployment

### Option 1: Manual Upload
- Upload new files via hosting provider's interface
- Replace old files with new ones

### Option 2: Git Integration
- Push changes to connected Git repository
- Hosting provider auto-deploys (Netlify, Vercel)

### Option 3: FTP/SFTP
- Connect to server
- Upload updated files
- Files are live immediately

## Troubleshooting Deployment

### Widget Not Appearing
- Check browser console for errors
- Verify all files uploaded correctly
- Check file paths are correct

### Knowledge Base Not Loading
- Verify `knowledge-base.json` is accessible
- Check file path in initialization code
- Test direct URL: `https://yoursite.com/widget/knowledge-base.json`

### 404 Errors
- Verify file structure matches documentation
- Check case sensitivity (Linux servers are case-sensitive)
- Ensure `widget/` folder exists

### Voice Not Working
- Ensure site is on HTTPS
- Check browser console for permission errors
- Test in Chrome or Safari (best support)

## Recommended Hosting Comparison

| Provider | Free Tier | HTTPS | CDN | Git Integration | Ease of Use |
|----------|-----------|-------|-----|-----------------|-------------|
| Netlify  | ✅ Yes    | ✅    | ✅  | ✅              | ⭐⭐⭐⭐⭐ |
| Vercel   | ✅ Yes    | ✅    | ✅  | ✅              | ⭐⭐⭐⭐ |
| GitHub Pages | ✅ Yes | ✅    | ✅  | ✅              | ⭐⭐⭐ |
| Traditional | ❌ No  | ⚠️    | ⚠️  | ❌              | ⭐⭐ |

## Support

For deployment issues:
- Check hosting provider documentation
- Review browser console for errors
- Contact: partner@jmminnovations.com

---

**Last Updated:** December 2024

