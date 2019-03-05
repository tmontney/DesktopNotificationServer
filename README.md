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

Note: At this time, clicking the notification isn't terribly useful. Once it's dismissed, clicking does nothing. The Action Center and Balloon Tips are not connected to each other.
