using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MySims
{
    /// <summary>
    /// Sonido ambiente y musica 100% procedural (sin archivos de audio).
    /// Genera tonos con AudioClip.Create al arrancar y los reproduce:
    /// campanada al cambiar la hora, alerta de necesidad critica, monedas al
    /// ganar dinero, fanfarria de logro, sonido de uso por tipo de mueble y
    /// una almohadilla musical de fondo que cambia de tono entre dia y noche.
    /// Silenciable con el boton "Sonido" o llamando a ToggleMute().
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Range(0f, 1f)] public float sfxVolume = 0.6f;
        [Range(0f, 1f)] public float musicVolume = 0.25f;
        public static bool Muted { get; private set; }

        readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
        AudioSource sfxSource;
        AudioSource musicSource;
        float lastCriticalBeep = -10f;
        float lastMoneySound = -10f;

        void Awake()
        {
            Instance = this;
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.volume = musicVolume;

            // --- Clips generados por codigo ---
            clips["click"] = MakeTone("click", 0.06f, 880f, 0.5f, WaveShape.Square);
            clips["chime"] = MakeTone("chime", 0.35f, 660f, 0.35f, WaveShape.Sine, 0.9f);
            clips["beep"] = MakeTone("beep", 0.25f, 520f, 0.6f, WaveShape.Triangle);
            clips["coins"] = MakeTone("coins", 0.18f, 1180f, 0.4f, WaveShape.Sine, 0.8f);
            clips["fanfare"] = MakeChord("fanfare", 0.8f, new[] { 523f, 659f, 784f }, 0.3f, WaveShape.Sine);
            clips["travel"] = MakeTone("travel", 0.5f, 240f, 0.45f, WaveShape.Sawtooth, 1.4f);
            clips["use_Hambre"] = MakeTone("use_Hambre", 0.2f, 330f, 0.4f, WaveShape.Sine, 1.2f);
            clips["use_Energia"] = MakeTone("use_Energia", 0.3f, 196f, 0.4f, WaveShape.Sine, 1.1f);
            clips["use_Higiene"] = MakeNoise("use_Higiene", 0.4f, 0.25f);
            clips["use_Social"] = MakeTone("use_Social", 0.2f, 440f, 0.35f, WaveShape.Sine, 1.3f);
            clips["use_Diversión"] = MakeTone("use_Diversión", 0.25f, 587f, 0.4f, WaveShape.Triangle, 1.5f);
            clips["use_Logica"] = MakeTone("use_Logica", 0.2f, 494f, 0.35f, WaveShape.Sine, 1.2f);

            // Almohadilla de fondo: acorde mayor de dia, menor de noche (8 s en loop)
            clips["pad_dia"] = MakeChord("pad_dia", 8f, new[] { 130.8f, 164.8f, 196f, 329.6f }, 0.16f, WaveShape.Sine, true);
            clips["pad_noche"] = MakeChord("pad_noche", 8f, new[] { 110f, 130.8f, 164.8f, 261.6f }, 0.14f, WaveShape.Sine, true);
        }

        void Start()
        {
            if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged += OnHour;
            if (NeedsSystem.Instance != null) NeedsSystem.Instance.OnNeedChanged += OnNeed;
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged += OnMoney;
            if (AchievementSystem.Instance != null) AchievementSystem.Instance.OnUnlocked += OnAchievement;
            if (TravelSystem.Instance != null) TravelSystem.Instance.OnTravel += OnTravel;
            UpdateAmbient();
            musicSource.Play();
        }

        void OnDestroy()
        {
            if (TimeSystem.Instance != null) TimeSystem.Instance.OnHourChanged -= OnHour;
            if (NeedsSystem.Instance != null) NeedsSystem.Instance.OnNeedChanged -= OnNeed;
            if (EconomySystem.Instance != null) EconomySystem.Instance.OnMoneyChanged -= OnMoney;
            if (AchievementSystem.Instance != null) AchievementSystem.Instance.OnUnlocked -= OnAchievement;
            if (TravelSystem.Instance != null) TravelSystem.Instance.OnTravel -= OnTravel;
        }

        // ---------- API publica ----------

        public static void ToggleMute()
        {
            Muted = !Muted;
            if (Instance == null) return;
            AudioListener.volume = Muted ? 0f : 1f;
        }

        public static void PlayClick()
        {
            if (!Muted) Instance?.sfxSource.PlayOneShot(Instance.clips["click"], Instance.sfxVolume);
        }

        /// <summary>Sonido de "empezar a usar un mueble", elegido por la necesidad que satisface.</summary>
        public static void PlayUse(NeedType need)
        {
            if (Muted) return;
            if (Instance == null || Instance.sfxSource == null) return;
            if (Instance.clips.TryGetValue("use_" + need, out var clip))
                Instance.sfxSource.PlayOneShot(clip, Instance.sfxVolume);
            else
                Instance.sfxSource.PlayOneShot(Instance.clips["click"], Instance.sfxVolume);
        }

        // ---------- eventos ----------

        void OnHour(int hour)
        {
            if (hour == 7 || hour == 22) sfxSource.PlayOneShot(clips["chime"], sfxVolume);
            UpdateAmbient();
        }

        void OnNeed(Need need)
        {
            if (!need.IsCritical) return;
            // Beep espaciado para no saturar
            if (Time.unscaledTime - lastCriticalBeep < 8f) return;
            lastCriticalBeep = Time.unscaledTime;
            sfxSource.PlayOneShot(clips["beep"], sfxVolume);
        }

        void OnMoney(float newBalance)
        {
            if (Time.unscaledTime - lastMoneySound < 1f) return;
            lastMoneySound = Time.unscaledTime;
            sfxSource.PlayOneShot(clips["coins"], sfxVolume * 0.6f);
        }

        void OnAchievement(Achievement a) => sfxSource.PlayOneShot(clips["fanfare"], sfxVolume);

        void OnTravel(LocationZone from, LocationZone to) => sfxSource.PlayOneShot(clips["travel"], sfxVolume);

        void UpdateAmbient()
        {
            var t = TimeSystem.Instance;
            bool night = t != null && t.IsNight;
            var pad = night ? clips["pad_noche"] : clips["pad_dia"];
            if (musicSource.clip != pad)
            {
                musicSource.clip = pad;
                if (musicSource.isPlaying) musicSource.Play();
            }
        }

        // ---------- generacion procedural de clips ----------

        enum WaveShape { Sine, Square, Triangle, Sawtooth }

        const int SampleRate = 22050;

        static float Sample(WaveShape shape, float phase)
        {
            switch (shape)
            {
                case WaveShape.Square: return phase < 0.5f ? 1f : -1f;
                case WaveShape.Triangle: return 4f * Mathf.Abs(phase - 0.5f) - 1f;
                case WaveShape.Sawtooth: return 2f * phase - 1f;
                default: return Mathf.Sin(phase * Mathf.PI * 2f);
            }
        }

        static AudioClip MakeTone(string name, float seconds, float freq, float volume, WaveShape shape, float pitchDrop = 1f)
        {
            int samples = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float f = freq * Mathf.Pow(pitchDrop, t); // sube o baja el tono a lo largo del clip
                float phase = (f * i / SampleRate) % 1f;
                float env = Mathf.Sin(t * Mathf.PI); // ataque y salida suaves
                data[i] = Sample(shape, phase) * env * volume;
            }
            return CreateClip(name, data);
        }

        static AudioClip MakeChord(string name, float seconds, float[] freqs, float volume, WaveShape shape, bool fadeLoop = false)
        {
            int samples = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float env = fadeLoop ? 1f : Mathf.Sin(t * Mathf.PI);
                float sum = 0f;
                foreach (var f in freqs)
                {
                    float phase = (f * i / SampleRate) % 1f;
                    sum += Sample(shape, phase);
                }
                // En loop suavizamos el borde para evitar el clic al repetir
                float edge = Mathf.Min(1f, Mathf.Min(i, samples - 1 - i) / (float)SampleRate * 20f);
                data[i] = sum / freqs.Length * env * volume * (fadeLoop ? edge : 1f);
            }
            return CreateClip(name, data);
        }

        static AudioClip MakeNoise(string name, float seconds, float volume)
        {
            var rng = new System.Random(1234);
            int samples = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[samples];
            float prev = 0f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float white = (float)(rng.NextDouble() * 2.0 - 1.0);
                prev = 0.94f * prev + 0.06f * white; // ruido "marron": suena a agua
                data[i] = prev * 6f * Mathf.Sin(t * Mathf.PI) * volume;
            }
            return CreateClip(name, data);
        }

        static AudioClip CreateClip(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }

    /// <summary>Boton para silenciar/activar el sonido. Colocar en un Button.</summary>
    [RequireComponent(typeof(Button))]
    public class AudioToggleUI : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                AudioManager.ToggleMute();
                AudioManager.PlayClick();
            });
        }
    }
}
