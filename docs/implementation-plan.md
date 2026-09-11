# Implementation Plan

- Add a standalone Angular client under `ClientApp`.
- Model conversation entries with a typed `ChatMessage` interface.
- Isolate `POST /api/hr/ask` in `HrAssistantService`.
- Build one focused chat component with reactive input validation, suggestions, loading, errors, keyboard submit, and auto-scroll.
- Add responsive styling for desktop and mobile layouts.
- Allow the Angular development origin through the ASP.NET API CORS policy.
- Verify with the Angular unit tests, Angular production build, and the ASP.NET build.

Acceptance is aligned with `.github/docs/hr-assistant-ui.md`: the client starts, shows the welcome state, sends non-empty questions as JSON strings, renders `answer`, handles loading/errors, and remains usable on narrow screens.
