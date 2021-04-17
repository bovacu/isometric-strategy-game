using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour {

    [Header("Console")] 
    [SerializeField] private GameObject fullContainer;
    [SerializeField] private TMP_InputField commandLine;
    [SerializeField] private GameObject logPanel;
    [SerializeField] private GameObject logContent;
    [SerializeField] private Button logButton;
    [SerializeField] private GameObject commandResultPrefab;
    [SerializeField] private GameObject helpCommandPrefab;

    [SerializeField] private float normalCommandHeight;
    [SerializeField] private float helpCommandHeight;
    
    [Header("Mono Behaviours")]
    [SerializeField] private MapControls controls;
    [SerializeField] private PlayerData playerData;

    [Header("Helpers")] 
    [SerializeField] private bool allCheats;
    
    private List<string> validCommands = new List<string> {"select", "help", "clean", "close", "player", "infinite"};
    private List<string> commandHistory = new List<string>();

    private int commandPointer = 0;

    public static bool consoleActive = false;
    
    public static bool infiniteEnergy = false;
    public static bool infiniteHealth = false;

    private void Start() {
        fullContainer.SetActive(false);
        commandLine.onSubmit.AddListener(executeCommand);
        logButton.onClick.AddListener(close);

        if (allCheats)
            infiniteAll();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F2)) {
            fullContainer.SetActive(!commandLine.gameObject.activeInHierarchy);
            
            consoleActive = commandLine.gameObject.activeInHierarchy;
            
            if (commandLine.gameObject.activeInHierarchy) {
                commandLine.ActivateInputField();
                logPanel.SetActive(logContent.transform.childCount > 0);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Escape)) {
            fullContainer.SetActive(false);
            consoleActive = commandLine.gameObject.activeInHierarchy;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (commandHistory.Any()) {
                if (commandPointer - 1 >= 0) {
                    commandPointer--;
                    commandLine.text = commandHistory[commandPointer];
                    commandLine.ActivateInputField();
                    commandLine.caretPosition = commandLine.text.Length;
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (commandHistory.Any()) {
                if (commandPointer + 1 < commandHistory.Count) {
                    commandPointer++;
                    commandLine.text = commandHistory[commandPointer];
                    commandLine.ActivateInputField();
                    commandLine.caretPosition = commandLine.text.Length;
                }
            }
        }
    }

    private void executeCommand(string _command) {
        logPanel.SetActive(true);
        
        var _subcommands = _command.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        
        commandHistory.Add(_command);
        commandPointer = commandHistory.Count;

        var _bigCommands = commandHistory.FindAll(_c => _c.Contains("help")).Count;
        
        logContent.GetComponent<RectTransform>().sizeDelta = new Vector2(logContent.GetComponent<RectTransform>().sizeDelta.x,
            logContent.transform.childCount * (normalCommandHeight + 1) + _bigCommands * helpCommandHeight);
        
        foreach (var _subcommand in _subcommands) {
            
            var _arguments = _subcommand.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            if (_arguments.Length == 0 || !validCommands.Contains(_arguments[0].ToLower())) {
                commandLine.text = "";
                
                if(!validCommands.Contains(_arguments[0].ToLower())) 
                    addError($"{_arguments[0]} is not a valid command, type 'help' to check available commands");

                commandLine.ActivateInputField();
                return;
            }

            helpCommand(_arguments);
            selectCommand(_arguments);
            closeCommand(_arguments);
            cleanCommand(_arguments);
            playerCommands(_arguments);
            cheatCommands(_arguments);
        }

        commandLine.text = "";
        commandLine.ActivateInputField();
    }

    private void selectCommand(string[] _arguments) {
        if (_arguments[0].ToLower().Equals("select")) {

            if (_arguments.Length != 3)
                return;

            try {
                var _x = int.Parse(_arguments[1]);
                var _y = int.Parse(_arguments[2]);
            
                controls.selectTile(new Vector2(_x, _y));
                
                addResult($"Selected cell ({_x}, {_y})");
            } catch (Exception _e) {
                addError("command is 'select x y' with x and y integers");
            }
        }
    }

    private void helpCommand(string[] _arguments) {
        if(_arguments[0].ToLower().Equals("help")) {
            addResult("", "help");
        }
    }
    
    private void closeCommand(string[] _arguments) {
        if(_arguments[0].ToLower().Equals("close"))
            fullContainer.SetActive(false);
    }
    
    private void cleanCommand(string[] _arguments) {
        if(_arguments[0].ToLower().Equals("clean")) {
            commandPointer = 0;
            logContent.GetComponent<RectTransform>().sizeDelta = new Vector2(logContent.GetComponent<RectTransform>().sizeDelta.x, 30);
            foreach (Transform _child in logContent.transform) {
                Destroy(_child.gameObject);
            }
        }
    }

    private void playerCommands(string[] _arguments) {
        healthCommand(_arguments);
        energyCommand(_arguments);
    }
    
    private void energyCommand(string[] _arguments) {
        if (_arguments[0].ToLower().Equals("player") && _arguments[1].ToLower().Equals("energy")) {

            if (_arguments.Length != 3)
                return;

            try {
                var _x = int.Parse(_arguments[2]);
                playerData.updateEnergy(_x);
                
                addResult($"Player energy {_x}");
            } catch (Exception _e) {
                Debug.Log(_e);
                addError("command is 'player energy x' with x the new energy amount");
            }
        }
    }
    
    private void healthCommand(string[] _arguments) {
        if (_arguments[0].ToLower().Equals("player") && _arguments[1].ToLower().Equals("health")) {

            if (_arguments.Length != 3)
                return;

            try {
                var _x = int.Parse(_arguments[2]);
                playerData.updateHealth(_x);
                
                addResult($"Player health {_x}");
            } catch (Exception _e) {
                Debug.Log(_e);
                addError("command is 'player health x' with x the new life amount");
            }
        }
    }
    
    private void cheatCommands(string[] _arguments) {
        if (_arguments[0].ToLower().Equals("infinite")) {

            if (_arguments[1].ToLower().Equals("all")) {
                infiniteAll();
            } else {
                if (_arguments[1].ToLower().Equals("energy")) {
                    infiniteEnergy = _arguments[2].ToLower().Equals("on");
                } else if (_arguments[1].ToLower().Equals("health")) {
                    infiniteHealth = _arguments[2].ToLower().Equals("on");
                }
            }
            
            addResult($"Infinite {_arguments[1]} {_arguments[2]}");
        }
    }

    private void infiniteAll() {
        infiniteEnergy = true;
        infiniteHealth = true;
    }
    
    private void close() {
        logPanel.SetActive(false);
    }

    private void addResult(string _res, string _commandForBiggerText = null) {
        if (_commandForBiggerText != null) {
            if (_commandForBiggerText.ToLower().Equals("help")) {
                Instantiate(helpCommandPrefab, logContent.transform);
            }
        } else {
            var _result = Instantiate(commandResultPrefab, logContent.transform);
            _result.GetComponent<TextMeshProUGUI>().text = $"> {_res}";   
        }
    }

    private void addError(string _error) {
        var _result = Instantiate(commandResultPrefab, logContent.transform);
        _result.GetComponent<TextMeshProUGUI>().text = $"> Error: {_error}";
        _result.GetComponent<TextMeshProUGUI>().color = Color.red;
    }
}