package com.loadly.driver

import android.content.Intent
import android.os.Bundle
import io.flutter.embedding.android.FlutterFragmentActivity
import io.flutter.embedding.android.RenderMode
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel

class MainActivity : FlutterFragmentActivity() {
    companion object {
        private const val CHANNEL = "com.loadly.driver/notification_launch"
        private const val LAUNCH_MARKER = "_fromNotificationLaunch"
    }

    private var pendingNotificationIntent: Intent? = null

    override fun getRenderMode(): RenderMode {
        return RenderMode.texture
    }

    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)
        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, CHANNEL)
            .setMethodCallHandler { call, result ->
                when (call.method) {
                    "consumeLaunchNotification" -> {
                        val source = pendingNotificationIntent ?: intent
                        if (!isNotificationIntent(source)) {
                            pendingNotificationIntent = null
                            result.success(null)
                            return@setMethodCallHandler
                        }

                        val data = extractNotificationData(source)?.toMutableMap()
                            ?: mutableMapOf()
                        data[LAUNCH_MARKER] = "true"
                        // Drop the launch intent after one read. Otherwise every
                        // resume — including returning from the camera — treats
                        // it as a new notification tap and reloads the trip page.
                        clearNotificationIntent()
                        android.util.Log.d(
                            "MainActivity",
                            "consumeLaunchNotification data=$data",
                        )
                        result.success(data)
                    }
                    else -> result.notImplemented()
                }
            }
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        captureNotificationIntent(intent)
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        setIntent(intent)
        captureNotificationIntent(intent)
    }

    private fun clearNotificationIntent() {
        pendingNotificationIntent = null
        val cleaned = Intent(Intent.ACTION_MAIN)
        cleaned.component = intent?.component
        setIntent(cleaned)
    }

    private fun captureNotificationIntent(intent: Intent?) {
        if (isNotificationIntent(intent)) {
            pendingNotificationIntent = intent
            android.util.Log.d(
                "MainActivity",
                "Captured notification intent action=${intent?.action}",
            )
        }
    }

    private fun isNotificationIntent(intent: Intent?): Boolean {
        if (intent == null) return false
        if (intent.action == "FLUTTER_NOTIFICATION_CLICK") return true
        val extras = intent.extras ?: return false
        if (extras.containsKey("google.message_id")) return true
        if (extras.containsKey("clickAction") || extras.containsKey("click_action")) {
            return true
        }
        if (extras.containsKey("resourceId") || extras.containsKey("resource_id")) {
            return true
        }
        return false
    }

    private fun extractNotificationData(intent: Intent?): Map<String, String>? {
        if (intent == null || !isNotificationIntent(intent)) return null
        val extras = intent.extras ?: return null
        val data = mutableMapOf<String, String>()
        for (key in extras.keySet()) {
            if (key == null) continue
            val value = extras.get(key)?.toString() ?: continue
            when {
                key.startsWith("google.") -> continue
                key.startsWith("gcm.notification.") -> continue
                key == "from" || key == "collapse_key" -> continue
                else -> data[key] = value
            }
        }
        return if (data.isEmpty()) null else data
    }
}
