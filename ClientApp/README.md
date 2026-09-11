# AI HR Assistant client

This Angular standalone app provides the HR chat experience described in `.github/docs/hr-assistant-ui.md`.

## Run locally

```powershell
npm install
npm start
```

Open `http://localhost:4200`. The API must be running at `http://localhost:5118`.

## Validate

```powershell
npm test
npm run build
```

The API request body is intentionally a JSON string, matching `POST /api/hr/ask`.
