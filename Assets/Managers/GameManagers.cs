using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers : MonoBehaviour
{
    // Singleton instance
    public static GameManagers instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // DontDestroyOnLoad(gameObject); // Optional: persists across scenes
    }

    [Header("Inventory Management")]
    public CursorTooltip cursorTooltip;

    [SerializeField] private GameState gameState;

    // Time threshold to differentiate between tap and hold (in seconds)
    [SerializeField] private float holdThreshold = 0.3f;

    private void Start()
    {
        SetGameState(gameState);
    }

    public void SetGameState(GameState newState)
    {
        gameState = newState;

        switch (newState)
        {
            case GameState.Game_Only:
                // Disable tooltip and lock cursor for game-only state
                cursorTooltip.followMouse = false;
                cursorTooltip.HideTooltip();
                Cursor.visible = false;
                // Cursor.lockState = CursorLockMode.Locked;
                Debug.Log("GameOnly");

                // Return items to inventory if tooltip slot is occupied
                if (cursorTooltip.slot.items != null)
                {
                    cursorTooltip.AddBackToSlot();
                }
                break;

            case GameState.UI_Only:
                // Enable tooltip and unlock cursor for UI-only state
                
                cursorTooltip.followMouse = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("UI_Only");
                break;
        }
    }

    private void Update()
    {
        if (gameState == GameState.UI_Only && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Start the touch timer when the touch begins
                    cursorTooltip.touchStartTime = Time.time;
                    break;

                case TouchPhase.Moved:
                    // If the touch is moving, don't show the tooltip (we treat it as a hold)
                    if (Time.time - cursorTooltip.touchStartTime > holdThreshold)
                    {
                        cursorTooltip.HideTooltip(); // Hide the tooltip if the touch is a hold
                    }
                    break;

                case TouchPhase.Ended:
                    // Check if the touch duration was short (a tap)
                    if (Time.time - cursorTooltip.touchStartTime <= holdThreshold)
                    {
                        if (!cursorTooltip.IsTouchInsideTooltip(touch.position))
                        {
                            // Hide the tooltip if the touch is outside the tooltip area
                            cursorTooltip.HideTooltip();
                        }
                        else
                        {
                            // Show the tooltip on tap (inside tooltip area)
                            cursorTooltip.DisplayTooltip("Sample Item", "Sample Description", 100); // Replace with actual item data
                        }
                    }
                    break;
            }
        }
    }
}

public enum GameState { UI_Only, Game_Only }
