# Bill Checker v3
- Environment: .NET 9.0
- Blazor Server/Static: Main, Controller,...
- Blazor Webassembly: Component
- MAUI Blazor Hybrid App: Main mobile
- Libraries: Models, Entities, Extensions,...

# Blazor 
- Create mirations: ```dotnet ef migrations add Init -o Data/Migrations```
- Create database: ```dotnet ef database update```
- Publish project: ```dotnet publish -c Release --output ./Publish TaxiNT.csproj```

# MAUI Blazor
- Publish Android app: https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-9.0
- Create Key: ```keytool -genkeypair -v -keystore my.keystore -alias TaxiNTKeyName -keyalg RSA -keysize 2048 -validity 10000```
- Find : ```keytool -list -keystore my.keystore```
- Publish: ```dotnet publish -f net9.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=my.keystore -p:AndroidSigningKeyAlias=TaxiNTKeyName -p:AndroidSigningKeyPass=110894 -p:AndroidSigningStorePass=110894```

# GoogleSheet Databases Extension
- Create project: [Google Cloud Console - Service Accounts](https://console.cloud.google.com/iam-admin/serviceaccounts)
- Register service
- Create API
- Down file Json account, save name: ```ggsheetaccount.json```
  
# Tailwind layout
- Docs: https://tailwindcss.com/docs/installation/tailwind-cli
- Local run cmd: ```../ntchecker-v2``` 
- Install Tailwind CSS: ```npm install tailwindcss @tailwindcss/cli```
- Build process for WEB: ```npx @tailwindcss/cli -i ./TaxiNT/TailwindImport/input.css -o ./TaxiNT/wwwroot/css/tailwindcss.css --watch```
- Build process for Mobile: ```npx @tailwindcss/cli -i ./TaxiNT.MAUI/TailwindImport/input.css -o ./TaxiNT.MAUI/wwwroot/css/tailwindcss.css --watch```
