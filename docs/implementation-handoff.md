# Implementation Handoff

The Angular client lives in `ClientApp` and is independent of the ASP.NET project build.

Key files:

- `src/app/components/hr-assistant/`: UI and conversation behavior.
- `src/app/services/hr-assistant.service.ts`: API integration.
- `src/app/models/chat-message.model.ts`: message contract.
- `src/app/app.config.ts`: standalone providers.

Run the API with the existing `http` launch profile, then run `npm install` and `npm start` from `ClientApp`.
