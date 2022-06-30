using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    public BrightnessOverrideComponent BrightnessOverride;
    public GameObject Content;

    // Use this for initialization
    private void Start()
    {
        this.BrightnessOverride.Brightness = 0f;
        Observable.Interval(TimeSpan.FromSeconds(1f))
            .Subscribe(_ =>
            {
                if (BrightnessOverride.Brightness < 1f)
                {
                    BrightnessOverride.Brightness += 0.1f;
                }
                else if (BrightnessOverride.Brightness == 1f)
                {
                    BrightnessOverride.Brightness = 0f;
                }
            }).AddTo(this);
        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}