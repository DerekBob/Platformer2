using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

//TO BE CLEAR. We will change everything once every entity gets tracked. so prepare for a refactor
public class DebugConsole : MonoBehaviour
{
    [Header("Console Stuff")]
    public GameObject ConsolePanel;
    public TMP_InputField inputField;
    public KeyCode toggleKey = KeyCode.BackQuote; // @/~ key
    private bool isConsoleOpen = false;
    [SerializeField] private ConsoleLogger log;

    [Header("Debug Panels")]
    public GameObject controllerStickPanel;

    private UnityEvent onEnter = new UnityEvent();

    private string inputText;

    [Header("Command History")]
    [SerializeField] private CommandHistory history = new CommandHistory(10);

    //private 

    // Start is called before the first frame update
    void Start()
    {
        ConsolePanel.SetActive(isConsoleOpen);
        onEnter.AddListener(ProcessCommand);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(toggleKey))
        {
            isConsoleOpen = !isConsoleOpen;
            ConsolePanel.SetActive(isConsoleOpen);

            if (isConsoleOpen)
            {
                history.ResetCursor();

            }
        }

        if (isConsoleOpen)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                inputField.text = history.NavigateUp();
                inputField.MoveTextEnd(false);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                inputField.text = history.NavigateDown();
                inputField.MoveTextEnd(false);
            }

        }

    }

    public void GrabFromInputField(string input)
    {
        inputText = input.Trim();
        
        inputField.text = "";

        history.Push(inputText);

        onEnter.Invoke();
    }

    void ProcessCommand()
    {
        if (string.IsNullOrWhiteSpace(inputText))
        {
            //Debug.Log("Null String");
            return;
        }
            
        //Debug.Log(inputText);
        string[] parts = inputText.Split(' ');
        string command = parts[0];

        string[] args = new string[parts.Length - 1];
        for (int i = 1; i < parts.Length; i++)
            args[i - 1] = parts[i].ToLower();

        switch (command)
        {
            case "showControllerCalibration":

                HandleShowControllerCalibration(args);

                break;
            case "clear":

                log.ClearLog();

                break;
            case "help":

                HandleHelp();

                break;
            default:

                log.Message("Invalid Command. Type 'help' for a list of all available commands", ConsoleLogger.MessageType.Warning);
                break;
        }

    }

    void HandleShowControllerCalibration(string[] args)
    {
        if (args.Length != 1)
        {
            log.Message("Invalid number of args", ConsoleLogger.MessageType.Warning);
            return;
        }

        if (args[0] == "true")
        {
            log.Message("Showing Controller Sticks Visual", ConsoleLogger.MessageType.Info);
            controllerStickPanel.SetActive(true);
        }
        else if (args[0] == "false")
        {
            log.Message("Removing Controller Sticks Visual", ConsoleLogger.MessageType.Info);
            controllerStickPanel.SetActive(false);
        }
        else
        {
            log.Message("Removing Controller Sticks Visual", ConsoleLogger.MessageType.Warning);
        }
    }

    void HandleHelp()
    {
        log.Message("Following list of commands:", ConsoleLogger.MessageType.Warning);
        log.Message("- showControllerCalibration [true|false]: will show your joystick or keypress", ConsoleLogger.MessageType.Info);
        log.Message("- clear: clear the log", ConsoleLogger.MessageType.Info);
    }

}

public class CommandHistory
{
    private List<string> history = new List<string>();
    private int maxSize;
    private int cursor = -1; // -1 = new command

    public CommandHistory(int maxSize = 50)
    {
        this.maxSize = maxSize;
    }

    public void Push(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        // Avoid pushing same as last command
        if (history.Count == 0 || history[history.Count - 1] != input)
            history.Add(input);

        if (history.Count > maxSize)
            history.RemoveAt(0);

        cursor = -1; // Reset cursor
    }

    public string NavigateUp()
    {
        if (history.Count == 0) return "";

        if (cursor < history.Count - 1)
            cursor++;

        return history[history.Count - 1 - cursor];
    }

    public string NavigateDown()
    {
        if (cursor > 0)
        {
            cursor--;
            return history[history.Count - 1 - cursor];
        }

        cursor = -1;
        return ""; // Back to empty input
    }

    public void ResetCursor()
    {
        cursor = -1;
    }
}