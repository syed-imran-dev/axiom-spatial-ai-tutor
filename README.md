Astra: AI-Driven 3D Virtual Assistant
High-Performance Native iOS + Unity 3D Integration + Node.js Backend
Technical Showcase: 13+ Years Engineering Experience
🚀 Overview
Astra is a cross-platform "Super-App" prototype demonstrating the seamless integration of Real-time AI Voice APIs with an embedded Unity 3D engine. This project solves the high-latency "back-and-forth" problem common in AI assistants by using an asynchronous, event-driven architecture.
Key Engineering Challenges Solved:
The Unity-Native Bridge: Bi-directional communication between Swift (Native) and C# (Unity) with zero memory leaks.
Streaming AI Pipeline: Implementing OpenAI GPT-4o streaming via Server-Sent Events (SSE) to minimize perceived latency.
Full-Stack Scalability: A stateless Node.js middleware designed for horizontal scaling and rapid deployment.
🏗️ System Architecture
“I build systems, not just apps.”
mermaid
graph TD
    A[Mobile App - SwiftUI] <-->|Method Channel / MessageHandler| B[Unity 3D Engine]
    A <-->|Socket.io / REST| C[Node.js Middleware]
    C <-->|gRPC / Streaming| D[AI APIs - OpenAI / ElevenLabs]
    C <-->|Real-time Sync| E[Firebase Firestore]
Use code with caution.

🛠️ Tech Stack & Design Patterns
Layer	Technology	Patterns Used
Frontend	SwiftUI	MVVM, Combine, Dependency Injection
3D Engine	Unity 2024.x	Observer Pattern, ScriptableObjects, Bridge Architecture
Backend	Node.js	Controller-Service Pattern, JWT Auth, Event Emitters
Cloud	Google Cloud / Firebase	Serverless Functions, Real-time DB
🧩 Featured Implementation: The Native-Unity Bridge
A common bottleneck in hybrid apps is UI thread blocking. This implementation uses a Thread-Safe Message Queue to ensure iOS frames remain at 60fps while processing 3D data and AI responses simultaneously.
swift
// Native iOS side: Sending AI Intent to the Unity Character
func sendIntentToUnity(action: String) {
    UnityFramework.getInstance().sendMessageToGO(
        "AstraCharacter", 
        methodName: "OnAIActionReceived", 
        message: action
    )
}
Use code with caution.

👨‍💻 About the Architect
Syed Imran Ahmed | Senior Systems Architect
With 13+ years of professional engineering, I specialize in bridging legacy native performance with modern AI and immersive 3D experiences. This repository serves as a public demonstration of the clean, maintainable code standards I bring to private enterprise projects.
