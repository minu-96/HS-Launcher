using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [Header("플레이리스트")]
    public List<Track> playlist = new List<Track>();

    [Header("UI 연결")]
    public Image coverImage;        // 곡 아이콘
    public Text titleText;          // 곡 제목 (TMP면 TMP_Text로 변경)
    public Image playPauseImage;    // 재생/일시정지 버튼의 Image 컴포넌트
    public Sprite playSprite;
    public Sprite pauseSprite;

    [Header("설정")]
    public bool autoPlayOnStart = true;
    public bool loopPlaylist = true;

    private AudioSource source;
    private int currentIndex = 0;
    private bool wasPlaying = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
    }

    void Start()
    {
        if (playlist.Count == 0) return;
        LoadTrack(currentIndex);
        if (autoPlayOnStart) Play();
        else UpdateUI();
    }

    void Update()
    {
        if (source.clip != null && !source.isPlaying && wasPlaying && source.time == 0f)
        {
            wasPlaying = false;
            Next();
        }
        if (source.isPlaying) wasPlaying = true;
    }

    void LoadTrack(int index)
    {
        if (playlist.Count == 0) return;
        currentIndex = (index + playlist.Count) % playlist.Count;
        source.clip = playlist[currentIndex].clip;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playlist.Count == 0) return;
        Track t = playlist[currentIndex];
        if (coverImage != null) coverImage.sprite = t.cover;
        if (titleText != null) titleText.text = t.title;
        if (playPauseImage != null)
            playPauseImage.sprite = source.isPlaying ? pauseSprite : playSprite;
    }

    public void Play()
    {
        if (source.clip == null) LoadTrack(currentIndex);
        source.Play();
        wasPlaying = true;
        UpdateUI();
    }

    public void Pause()
    {
        source.Pause();
        wasPlaying = false;
        UpdateUI();
    }

    public void TogglePlayPause()
    {
        if (source.isPlaying) Pause();
        else Play();
    }

    public void Next()
    {
        int next = currentIndex + 1;
        if (next >= playlist.Count)
        {
            if (!loopPlaylist) { Pause(); return; }
            next = 0;
        }
        LoadTrack(next);
        Play();
    }

    public void Previous()
    {
        // 3초 이상 재생됐으면 현재 곡 처음으로
        if (source.time > 3f)
        {
            source.time = 0f;
            Play();
            return;
        }

        int prev = currentIndex - 1;
        if (prev < 0)
        {
            if (!loopPlaylist) { source.time = 0f; Play(); return; }
            prev = playlist.Count - 1;
        }
        LoadTrack(prev);
        Play();
    }
}