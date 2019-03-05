# DesktopNotificationServer
A basic example of posting notifications to the action center via network.

Runs over HTTP. This application combines NotifyIcon and Grapevine. Only security is adding IPs to its whitelist.

Usage: 

  Verb: POST
  
  Endpoint: {SERVER_IP}:30000/API/NOTIFICATION
  
  Body: JSON 
  
    {
    "title":"Application",
    "body": "Hello World!",
    "severity": 1
    }
