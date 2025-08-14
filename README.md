# Rescue System Client: Desktop client for Rescue System application
Cross-platform desktop application for operators of the "Rescue System" monitoring system. The application provides a centralized interface for monitoring alarms in real time, managing users and devices linked to them.

<img width="2444" height="1344" alt="tg_image_1352848666" src="https://github.com/user-attachments/assets/377e070f-3646-46ef-a5cc-8513c3e352cf" />

WebAPI for Rescue System:
[Rescue System Web API](https://github.com/blugsam/Rescue-System-Web-API)

Domain model repository:
[Rescue System Class Library](https://github.com/blugsam/Rescue-System-Class-Library)

## Architecture overview
The application is built using the **Model-View-ViewModel (MVVM)** architectural pattern.

## Technologies Used

* **Platform** .NET Core 9

* **UI Framework:** AvaloniaUI

* **MVVM library:** CommunityToolkit.Mvvm

* **Real-time:** SignalR

## Getting Started
To get started with this project, follow these steps:

1) Clone the repository using the following command:
```bash
git clone https://github.com/blugsam/Rescue-System-Client
```

2) Set Up the configuration string on configuration window. Basically it is:
```bash
http://localhost:5107/
```

2) Build and run the application
```bash
dotnet run
```

## Overview
⚠️Monitor live alerts on the app's home page

<img width="900" height="700" alt="image" src="https://github.com/user-attachments/assets/28ff25be-6d2c-4b51-a5dd-19d2911d0102" />

🚵🏻‍♂️Register users in the system

<img width="900" height="700" alt="image" src="https://github.com/user-attachments/assets/5f44c4d6-c79e-4fdd-a812-3a0a4494160b" />

🚣🏻‍♂️Assign users to bracelets

<img width="900" height="700" alt="image" src="https://github.com/user-attachments/assets/20eb4fd5-da48-4afc-a763-985f716c69cf" />
