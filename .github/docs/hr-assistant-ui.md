# AI HR Assistant — Angular UI Specification

## 1. Objective

Build a modern, responsive **AI HR Assistant** screen in Angular.

The UI should behave similarly to ChatGPT:

* User enters an HR-related question.
* Angular sends the question to the backend API.
* AI response is displayed in the conversation.
* User can continue asking questions.
* Conversation should visually distinguish user and AI messages.
* Show loading state while waiting for the AI response.
* Handle API errors gracefully.

---

# 2. Backend API

The backend API is already available.

### Endpoint

```text
POST http://localhost:5118/api/hr/ask
```

### Request

The API expects the question as a JSON string.

Example:

```json
"How many casual leaves can I take?"
```

Angular should send:

```http
POST http://localhost:5118/api/hr/ask
Content-Type: application/json
```

Request body:

```json
"How many casual leaves can I take?"
```

### Response

The API returns:

```json
{
  "answer": "Employees are eligible for..."
}
```

The Angular application must read:

```typescript
response.answer
```

and display it as the AI response.

---

# 3. Screen Layout

Create a full-screen HR Assistant application.

The screen should contain:

```text
 ---------------------------------------------------------
|                    AI HR Assistant                      |
|---------------------------------------------------------|
|                                                         |
|     AI: Hello! I'm your HR Assistant.                  |
|         How can I help you today?                       |
|                                                         |
|                         User: What is the leave policy? |
|                                                         |
|     AI: Our leave policy allows employees to...        |
|                                                         |
|                                                         |
|                                                         |
|---------------------------------------------------------|
|  Ask anything about HR...                    [ Send ]  |
 ---------------------------------------------------------
```

---

# 4. Header

Create a top navigation/header section.

Display:

### Application title

```text
AI HR Assistant
```

### Subtitle

```text
Your intelligent HR support assistant
```

Optionally include an AI/HR icon.

The header should remain visually clean and professional.

---

# 5. Chat Area

Create a scrollable chat container.

The chat area should display messages in chronological order.

Use two different message styles.

## User Message

User messages should:

* Appear aligned to the right.
* Have a visually distinct background.
* Display the user's question.
* Include a small "You" label or user icon.

Example:

```text
                         You
                         What is the leave policy?
```

## AI Message

AI messages should:

* Appear aligned to the left.
* Have a different background.
* Display an AI icon/avatar.
* Display the response from the API.

Example:

```text
AI Assistant

Our leave policy allows employees to take
casual, sick and earned leave based on the
company policy.
```

---

# 6. Initial Welcome Message

When the screen is opened, display:

```text
Hello! 👋

I'm your AI HR Assistant.

You can ask me questions about:

• Leave policies
• Attendance
• Payroll
• Employee benefits
• HR policies
• Company procedures

How can I help you today?
```

This message does not need to call the backend API.

---

# 7. Suggested Questions

Below the welcome message, display optional suggestion buttons.

Examples:

```text
What is the leave policy?
```

```text
How many sick leaves do I have?
```

```text
What are the working hours?
```

```text
How can I apply for leave?
```

When the user clicks a suggestion:

1. Put the question into the input box.
2. Send the question to the API.
3. Display the response.

---

# 8. Chat Input

At the bottom of the screen create a fixed chat input area.

Use:

```html
<input />
```

or a multi-line:

```html
<textarea></textarea>
```

Placeholder:

```text
Ask anything about HR...
```

Add a Send button.

Example:

```text
----------------------------------------------------
| Ask anything about HR...                    Send |
----------------------------------------------------
```

---

# 9. Sending a Question

When the user clicks **Send**:

### Step 1

Read the input value.

Example:

```typescript
const question = this.question.trim();
```

### Step 2

Do not send empty questions.

If the input is empty:

```text
Do nothing
```

### Step 3

Immediately add the user's question to the chat.

Example:

```text
You

What is the leave policy?
```

### Step 4

Clear the input.

### Step 5

Show loading state.

Example:

```text
AI Assistant

Thinking...
```

### Step 6

Call the backend API.

```http
POST http://localhost:5118/api/hr/ask
```

Request body:

```json
"What is the leave policy?"
```

### Step 7

Read the response.

```typescript
response.answer
```

### Step 8

Display the answer as an AI message.

---

# 10. Angular API Service

Create:

```text
src/app/services/hr-assistant.service.ts
```

The service should use Angular `HttpClient`.

Example structure:

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface HrAssistantResponse {
  answer: string;
}

@Injectable({
  providedIn: 'root'
})
export class HrAssistantService {

  private apiUrl = 'http://localhost:5118/api/hr/ask';

  constructor(private http: HttpClient) {}

  askQuestion(question: string): Observable<HrAssistantResponse> {
    return this.http.post<HrAssistantResponse>(
      this.apiUrl,
      question,
      {
        headers: {
          'Content-Type': 'application/json'
        }
      }
    );
  }
}
```

---

# 11. Chat Message Model

Create a model/interface for chat messages.

Example:

```typescript
export interface ChatMessage {
  role: 'user' | 'assistant';
  content: string;
  timestamp?: Date;
}
```

The component should maintain:

```typescript
messages: ChatMessage[] = [];
```

Example:

```typescript
messages = [
  {
    role: 'assistant',
    content: 'Hello! I am your AI HR Assistant.'
  }
];
```

---

# 12. Component

Create an Angular component:

```text
src/app/components/hr-assistant/
```

Suggested files:

```text
hr-assistant.component.ts
hr-assistant.component.html
hr-assistant.component.css
```

The component should:

* Display chat messages.
* Handle user input.
* Call `HrAssistantService`.
* Display loading state.
* Handle errors.
* Automatically scroll to the latest message.

---

# 13. Component Logic

Implement a method similar to:

```typescript
sendMessage(): void
```

Expected flow:

```text
User enters question
        ↓
Validate input
        ↓
Add user message
        ↓
Clear input
        ↓
Set loading = true
        ↓
Call HrAssistantService
        ↓
Receive response
        ↓
Add AI response
        ↓
Set loading = false
        ↓
Scroll to latest message
```

---

# 14. Loading State

While the API is processing the request, display:

```text
AI Assistant is thinking...
```

Optionally show animated dots:

```text
Thinking...
```

Example:

```text
● ● ●
```

Disable the Send button while the API request is running.

This prevents duplicate API requests.

---

# 15. Error Handling

If the API returns an error, display a friendly message.

Example:

```text
Sorry, I couldn't process your request right now.
Please try again.
```

Do not expose technical errors directly to the user.

Log technical errors to the browser console for development.

Example:

```typescript
error: (err) => {
  console.error('HR Assistant API error:', err);

  this.messages.push({
    role: 'assistant',
    content: 'Sorry, I could not process your request right now. Please try again.'
  });

  this.loading = false;
}
```

---

# 16. Enter Key Behavior

The input should support:

```text
Enter → Send message
```

For a textarea:

```text
Enter → Send
Shift + Enter → New line
```

Prevent sending when the input is empty.

---

# 17. Auto Scroll

Whenever a new message is added, automatically scroll the chat container to the bottom.

Expected behavior:

```text
New message
     ↓
Chat automatically scrolls
     ↓
Latest response is visible
```

---

# 18. Responsive Design

The application must work on:

* Desktop
* Laptop
* Tablet
* Mobile

Desktop:

```text
        ┌─────────────────────────────────┐
        │        AI HR Assistant          │
        │                                 │
        │         Chat Messages           │
        │                                 │
        │                                 │
        │ Ask anything...          Send   │
        └─────────────────────────────────┘
```

Mobile:

```text
┌──────────────────────┐
│ AI HR Assistant      │
├──────────────────────┤
│                      │
│ AI message           │
│                      │
│          User msg    │
│                      │
├──────────────────────┤
│ Ask anything...      │
│              [Send]  │
└──────────────────────┘
```

---

# 19. Visual Design

Use a modern enterprise AI design.

Requirements:

* Clean white/light background.
* Rounded chat bubbles.
* Subtle shadows.
* Good spacing.
* Professional typography.
* Clear hierarchy.
* Accessible contrast.
* Smooth scrolling.
* Responsive layout.

Avoid making the screen look like a traditional form.

It should feel like a modern AI assistant.

---

# 20. Suggested Angular Structure

Create the following structure:

```text
src/
└── app/
    ├── components/
    │   └── hr-assistant/
    │       ├── hr-assistant.component.ts
    │       ├── hr-assistant.component.html
    │       └── hr-assistant.component.css
    │
    ├── services/
    │   └── hr-assistant.service.ts
    │
    ├── models/
    │   └── chat-message.model.ts
    │
    ├── app.component.ts
    ├── app.component.html
    └── app.config.ts
```

If the Angular project uses a module-based architecture instead of standalone components, adapt the structure accordingly.

---

# 21. HttpClient Configuration

Make sure Angular `HttpClient` is configured.

For standalone Angular applications, use:

```typescript
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient()
  ]
};
```

---

# 22. CORS

The Angular application will call:

```text
http://localhost:5118
```

from a different development server/port.

Therefore, if the browser reports a CORS error, configure CORS in the ASP.NET Core backend to allow the Angular development URL.

Typical Angular development URL:

```text
http://localhost:4200
```

Do not change the API endpoint unless required.

---

# 23. API Contract

The frontend must follow this exact contract.

### Request

```http
POST /api/hr/ask
```

Body:

```json
"User question"
```

### Response

```json
{
  "answer": "AI generated answer"
}
```

Do not send:

```json
{
  "question": "User question"
}
```

unless the backend API is changed.

The current backend expects the request body to be a string.

---

# 24. Example Conversation

The UI should support conversations like:

```text
AI Assistant

Hello! 👋
I'm your AI HR Assistant.
How can I help you today?


You

What is the leave policy?


AI Assistant

Our company leave policy provides different
types of leave based on employee eligibility...


You

How do I apply for leave?


AI Assistant

You can apply for leave through the employee
portal by submitting a leave request...
```

---

# 25. Acceptance Criteria

The implementation is complete when:

* [ ] Angular application starts successfully.
* [ ] HR Assistant screen is displayed.
* [ ] Welcome message is displayed.
* [ ] User can enter an HR question.
* [ ] User can click Send.
* [ ] Empty messages are prevented.
* [ ] User message appears immediately.
* [ ] Loading state is displayed.
* [ ] API request is sent to:
  `http://localhost:5118/api/hr/ask`
* [ ] Request body is a JSON string.
* [ ] API response `answer` is displayed.
* [ ] User and AI messages have different styles.
* [ ] Send button is disabled while waiting.
* [ ] API errors are handled gracefully.
* [ ] Chat automatically scrolls to the latest message.
* [ ] Enter key sends the message.
* [ ] UI works on mobile and desktop.
* [ ] No hard-coded mock AI responses are used after API integration.
* [ ] Code follows Angular best practices.
* [ ] API communication is isolated inside an Angular service.

---

# 26. Development Instruction for the Coder Agent

Implement this UI in the existing Angular project.

Before modifying files:

1. Inspect the existing Angular project structure.
2. Identify whether the project uses standalone components or NgModules.
3. Reuse existing application configuration where appropriate.
4. Do not unnecessarily recreate the Angular project.
5. Implement the HR Assistant as a reusable component.
6. Create a dedicated API service.
7. Implement the API integration using `HttpClient`.
8. Implement responsive CSS.
9. Run/build the Angular application.
10. Fix compilation errors.
11. Verify the API request format.
12. Verify that the API response is rendered correctly.

Use this backend API:

```text
http://localhost:5118/api/hr/ask
```

Do not invent additional backend APIs.

---

# 27. Final Result

The final application should provide a ChatGPT-like HR experience:

```text
              AI HR Assistant
       Your intelligent HR support assistant

 ┌──────────────────────────────────────────────┐
 │                                              │
 │  🤖 Hello! I'm your AI HR Assistant.        │
 │     How can I help you today?                │
 │                                              │
 │                    You                       │
 │                    What is leave policy?     │
 │                                              │
 │  🤖 AI Assistant                             │
 │     Our leave policy provides...             │
 │                                              │
 │                                              │
 ├──────────────────────────────────────────────┤
 │ Ask anything about HR...             [Send]  │
 └──────────────────────────────────────────────┘
```

The implementation should be production-quality, responsive, accessible, and ready to connect to the existing ASP.NET Core HR Assistant API.
