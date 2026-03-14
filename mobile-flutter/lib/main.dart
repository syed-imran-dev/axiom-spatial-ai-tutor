import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:flutter_unity_widget/flutter_unity_widget.dart';
import 'services/api_service.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const MaterialApp(
    debugShowCheckedModeBanner: false,
    home: AxiomDashboard(),
  ));
}

class AxiomDashboard extends StatefulWidget {
  const AxiomDashboard({super.key});

  @override
  State<AxiomDashboard> createState() => _AxiomDashboardState();
}

class _AxiomDashboardState extends State<AxiomDashboard> {
  // Unity Controller to bridge Flutter and C#
  UnityWidgetController? _unityController;

  final AxiomApiService _api = AxiomApiService();
  final TextEditingController _input = TextEditingController();

  String _explanation = "Welcome to Axiom\nType a math problem below to see the 3D proof.";
  bool _isLoading = false;

  // Callback when Unity is ready
  void _onUnityCreated(UnityWidgetController controller) {
    _unityController = controller;
    debugPrint("Axiom: Unity Engine Linked Successfully.");
  }

  // Handle messages sent FROM Unity to Flutter (if needed)
  void _onUnityMessage(dynamic message) {
    debugPrint("Received from Unity: $message");
  }

  void _handleSolve() async {
    if (_input.text.trim().isEmpty) return;
    setState(() => _isLoading = true);

    try {
      final response = await _api.solveMath(_input.text);
      final payload = response['payload'];

      setState(() {
        _explanation = payload['text_solution']
            .toString()
            .replaceAll(RegExp(r'<[^>]*>'), '');
        _isLoading = false;
      });

      // --- THE 3D BRIDGE ---
      // Send the spatial data to the 'AxiomFactory' object in Unity
      if (_unityController != null) {
        // We wrap the objects list in the root 'objects' key Unity expects
        String spatialJson = jsonEncode({"objects": payload['objects']});

        _unityController!.postMessage(
          'AxiomFactory',     // Name of GameObject in Unity Hierarchy
          'BuildMathScene',    // Name of C# function in AxiomFactory.cs
          spatialJson,
        );
      }

    } catch (e) {
      setState(() => _isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text("Axiom Engine Error: $e")),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Axiom: Spatial Math Tutor"),
        backgroundColor: Colors.black87,
      ),
      body: Column(
        children: [
          // TOP: Live Unity 3D Visualization
          Expanded(
            flex: 4,
            child: Container(
              color: Colors.black,
              child: UnityWidget(
                onUnityCreated: _onUnityCreated,
                onUnityMessage: _onUnityMessage,
                fullscreen: false,
                // Important for Web/Android focus handling
                useAndroidViewSurface: true,
              ),
            ),
          ),

          // BOTTOM: AI Teacher Explanation & Input
          Expanded(
            flex: 6,
            child: Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: const BorderRadius.vertical(top: Radius.circular(25)),
                boxShadow: [
                  BoxShadow(color: Colors.black12, blurRadius: 10, spreadRadius: 2)
                ],
              ),
              child: Column(
                children: [
                  Expanded(
                    child: SingleChildScrollView(
                      child: SelectableText(
                        _explanation,
                        style: const TextStyle(fontSize: 16, height: 1.6, color: Colors.black87),
                      ),
                    ),
                  ),

                  if (_isLoading) const Padding(
                    padding: EdgeInsets.symmetric(vertical: 10),
                    child: LinearProgressIndicator(color: Colors.indigo),
                  ),

                  const SizedBox(height: 10),

                  TextField(
                    controller: _input,
                    onSubmitted: (_) => _handleSolve(),
                    style: const TextStyle(color: Colors.black),
                    decoration: InputDecoration(
                      hintText: "Enter a math problem...",
                      filled: true,
                      fillColor: Colors.grey[100],
                      hintStyle: const TextStyle(color: Colors.grey),
                      border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(15),
                          borderSide: BorderSide.none
                      ),
                      suffixIcon: Container(
                        margin: const EdgeInsets.all(6),
                        decoration: const BoxDecoration(
                          color: Colors.indigo,
                          shape: BoxShape.circle,
                        ),
                        child: IconButton(
                            icon: const Icon(Icons.send, color: Colors.white, size: 20),
                            onPressed: _handleSolve
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
