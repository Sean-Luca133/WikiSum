# WikiSum

## Prerequisites and Setup

Before running the application, you need to configure both the backend and the frontend.

### Backend Setup

Setting up the backend is straightforward. You only need one secret to get started: your ngrok authentication token.

1. Go to the [ngrok authtoken dashboard](https://dashboard.ngrok.com/get-started/your-authtoken) and create an account if you don't already have one.
2. Find your **AuthToken** on the dashboard and copy it.
3. Rename `.env.example` to `.env` and paste your AuthToken into the appropriate environment variable.
4. Run the following command from the root directory of the project:
   ```bash
   docker compose up --build -d
   ```
5. The backend is now up and running.

### Frontend / Application

Now that the backend is running, you need to configure the frontend by providing the required secrets and endpoints.

#### appsettings.example.json

Rename `appsettings.example.json` to `appsettings.json`.

#### Ngrok

To retrieve the public URL of your backend:

1. If you haven't changed the ngrok service port in the `compose.yml` file, you can access the ngrok dashboard at `http://localhost:4040/inspect/http`.
2. Find the public ngrok URL. It should look something like `https://random-name.ngrok-free.dev`.
3. Copy this URL and paste it into `appsettings.json` under the `BACKEND_API_ENDPOINT` setting.

#### Azure

This project uses two Azure services: Azure AI Translator and Azure AI Language. Both services are required for the application to function.

1. Create both services in the Azure Portal.
2. For the **Azure AI Language** service, you will need the following credentials:
   - `LANGUAGE_KEY`
   - `LANGUAGE_ENDPOINT`
3. For the **Azure AI Translator** service, you will need the following credentials:
   - `TRANSLATOR_KEY`
   - `TRANSLATOR_REGION`
   - `TRANSLATOR_ENDPOINT`

Retrieve the corresponding values from the Azure Portal and paste them into `appsettings.json` under the appropriate configuration settings.

## Running the Application

Once the backend and frontend are fully configured, you are ready to build and run the application.

### Build for Windows

Run the following command in PowerShell:

```powershell
.\run.ps1 Windows
```

### Build for android

Make sure your Android device is connected to your PC via USB-C and is properly recognized before running the command.

Run the following command in PowerShell:

```powershell
.\run.ps1 Android
```

## Showcase demo (PC + Phone build)

A demo of the use of the application and it's setup can be found here: [Demo WikiSum](https://www.youtube.com/watch?v=ilo_xinYZdw)