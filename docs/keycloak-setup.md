# Keycloak Setup Guide

## Quick Start

1. **Start Keycloak**:
   ```bash
   docker-compose up -d
   ```

2. **Access Keycloak Admin Console**:
   - URL: http://localhost:8080/admin
   - Username: `admin`
   - Password: `admin`

## Initial Configuration

### 1. Create a Realm

1. Log into the Keycloak admin console
2. Hover over the realm dropdown (top left, shows "master")
3. Click "Create Realm"
4. Name it: `cleanarch`
5. Click "Create"

### 2. Create a Client (Your API)

1. In the `cleanarch` realm, go to **Clients** (left sidebar)
2. Click **Create client**
3. **General Settings**:
   - Client type: `OpenID Connect`
   - Client ID: `cleanarch-api`
   - Click **Next**
4. **Capability config**:
   - Client authentication: `Off` (for public clients)
   - Authorization: `Off`
   - Standard flow: `On` (Authorization Code Flow)
   - Implicit flow: `On` (Implicit Flow)
   - Direct access grants: `On` (for testing with Postman/Swagger)
   - Click **Next**
5. **Login settings**:
   - Valid redirect URIs: `http://localhost:5038/*` (your API URL)
   - Web origins: `http://localhost:5038`
   - Click **Save**

### 3. Create a Test User

1. Go to **Users** (left sidebar)
2. Click **Create new user**
3. Fill in:
   - Username: `test`
   - Email: `testuser@example.com`
   - First name: `Test`
   - Last name: `User`
   - Email verified: `On`
4. Click **Create**
5. Go to the **Credentials** tab
6. Click **Set password**
7. Set password: `tester`
8. **Temporary**: `Off` (so user doesn't need to change password)
9. Click **Save**

### 4. Get Access Token (For Testing)

You can get a token using Keycloak's token endpoint:

```bash
curl -X POST http://localhost:8080/realms/cleanarch/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=cleanarch-api" \
  -d "username=test" \
  -d "password=tester" \
  -d "grant_type=password"
```

Response will include an `access_token` - use this in your API requests:

```bash
curl https://localhost:8443/api/user/profile \
  -H "Authorization: Bearer TOKEN"
```

## Testing with Swagger

1. Start your API: `dotnet run` (in `backend/CleanArch.API`)
2. Open Swagger: https://localhost:8443/swagger
3. Get a token using the curl command above
4. Click the **Authorize** button (top right in Swagger)
5. Enter: `Bearer YOUR_ACCESS_TOKEN_HERE`
6. Click **Authorize**
7. Now try the protected endpoints!

## Common Issues

### Token Validation Fails
- Check that the realm name matches: `cleanarch`
- Verify the client ID matches: `cleanarch-api`
- Ensure Keycloak is running: `docker ps`

### CORS Issues
- Make sure "Web origins" in client settings includes your API URL
- Check that `RequireHttpsMetadata` is `false` in `appsettings.json` for development

### Connection Refused
- Verify Keycloak is running: `docker-compose ps`
- Check logs: `docker-compose logs keycloak`
- Ensure port 8080 is not in use by another service

## Production Considerations

⚠️ **For production**, you should:
- Change admin credentials
- Use PostgreSQL instead of H2 database
- Enable HTTPS
- Set `RequireHttpsMetadata: true` in appsettings
- Use proper secrets management
- Configure proper CORS policies
- Set up proper realm and client security settings

