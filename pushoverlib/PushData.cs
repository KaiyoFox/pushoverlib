﻿using System.Runtime.CompilerServices;

namespace pushoverlib; 

public class PushData {
    // required
    public string? ApiToken { get; private set; }
    public string? UserToken { get; private set; }
    public string? Message { get; private set; }
    private bool set = false;
    // optional
    public readonly PushPriority? Priority;
    public readonly string? Title;
    public readonly string? Device;
    public readonly long? Timestamp;
    public readonly PushAttachment? Attachment;
    public readonly bool? Html;
    public readonly PushSound? Sound;
    public readonly PushUrl? Url;


    public enum PushPriority {
        Badge = -2,
        NotificationCenter = -1,
        Notify = 0,
        TimeSensitive = 1,
        RequireAck = 2
    }
    
    internal PushData AddNeeded(string apiToken, string userToken, string msg) {
        if (set) throw new InvalidOperationException("Already set");
        set = true;
        this.ApiToken = apiToken;
        this.UserToken = userToken;
        this.Message = msg;
        return this;
    }

    public Dictionary<string, string> ToDict() {
        var dict = new Dictionary<string, string>();

        void AddToDict(string key, string? val, bool req = false) {
            if (req && val == null) throw new InvalidOperationException("Required Field " + key + " not set");
            if (val == null) return;
            dict.Add(key, val);
        }
        void AddBoolToDict(string key, bool? val) {
            if (val == null) return;
            dict.Add(key, val.Value ? "1" : "0");
        }

        AddToDict("token", ApiToken, true);
        AddToDict("user", UserToken, true);
        AddToDict("message", Message, true);
        // don't add attachment because *pain* and it might be better to do in PushCommunicator
        if (Attachment?.IsBase64() ?? false) AddToDict("attachment_base64", Convert.ToBase64String(Attachment.Data));
        AddToDict("attachment_type", Attachment?.EvaluateType());
        AddToDict("device", Device);
        AddBoolToDict("html", Html);
        AddToDict("priority", Priority.ToString());
        AddToDict("sound", Sound?.Evaluate());
        AddToDict("timestamp", Timestamp?.ToString());
        AddToDict("title", Title);
        AddToDict("url", Url?.Url);
        AddToDict("url_title", Url?.Title);

        return dict;
    }

    public PushData(PushPriority? priority, string? title = null, string? device = null, long? timestamp = null, PushAttachment? attachment = null, bool? html = null, PushSound? sound = null, PushUrl? url = null) {
        Priority = priority;
        Title = title;
        Device = device;
        Timestamp = timestamp;
        Attachment = attachment;
        Html = html;
        Sound = sound;
        Url = url;
    }
}