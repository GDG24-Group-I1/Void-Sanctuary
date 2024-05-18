//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Settings/VoidSanctuaryActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @VoidSanctuaryActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @VoidSanctuaryActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""VoidSanctuaryActions"",
    ""maps"": [
        {
            ""name"": ""ControlsActionMap"",
            ""id"": ""94662d90-ea66-4bef-9fb6-b245da2f6dea"",
            ""actions"": [
                {
                    ""name"": ""ExitGameAction"",
                    ""type"": ""Button"",
                    ""id"": ""44a0eb93-0320-4c6c-8789-a2b2a809b92c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f682047e-d24b-4206-a1b2-903055f1a953"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5c627a58-cf1f-4b8e-a5c2-6e978de742f7"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""33eca119-57a7-4334-abdf-f9344a525c97"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""c312c83e-955d-4a01-b46b-9ef692209976"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""38fc0fd8-8869-4f44-a079-a758a170ba05"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""f9e80982-8ab4-4c1d-8058-bcbee4f9937f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""78affc91-d117-4378-9239-e4e5ad08aab2"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""584a269d-9610-48fe-8d8a-c0026bd4a577"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""3f630ea3-4517-4708-a64a-b8beb2cb31ad"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a59e8138-72c3-4f69-bab6-1bb14ef5bbe3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ControlsActionMap
        m_ControlsActionMap = asset.FindActionMap("ControlsActionMap", throwIfNotFound: true);
        m_ControlsActionMap_ExitGameAction = m_ControlsActionMap.FindAction("ExitGameAction", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // ControlsActionMap
    private readonly InputActionMap m_ControlsActionMap;
    private List<IControlsActionMapActions> m_ControlsActionMapActionsCallbackInterfaces = new List<IControlsActionMapActions>();
    private readonly InputAction m_ControlsActionMap_ExitGameAction;
    public struct ControlsActionMapActions
    {
        private @VoidSanctuaryActions m_Wrapper;
        public ControlsActionMapActions(@VoidSanctuaryActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ExitGameAction => m_Wrapper.m_ControlsActionMap_ExitGameAction;
        public InputActionMap Get() { return m_Wrapper.m_ControlsActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlsActionMapActions set) { return set.Get(); }
        public void AddCallbacks(IControlsActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_ControlsActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ControlsActionMapActionsCallbackInterfaces.Add(instance);
            @ExitGameAction.started += instance.OnExitGameAction;
            @ExitGameAction.performed += instance.OnExitGameAction;
            @ExitGameAction.canceled += instance.OnExitGameAction;
        }

        private void UnregisterCallbacks(IControlsActionMapActions instance)
        {
            @ExitGameAction.started -= instance.OnExitGameAction;
            @ExitGameAction.performed -= instance.OnExitGameAction;
            @ExitGameAction.canceled -= instance.OnExitGameAction;
        }

        public void RemoveCallbacks(IControlsActionMapActions instance)
        {
            if (m_Wrapper.m_ControlsActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IControlsActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_ControlsActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ControlsActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ControlsActionMapActions @ControlsActionMap => new ControlsActionMapActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    public struct PlayerActions
    {
        private @VoidSanctuaryActions m_Wrapper;
        public PlayerActions(@VoidSanctuaryActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IControlsActionMapActions
    {
        void OnExitGameAction(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
    }
}