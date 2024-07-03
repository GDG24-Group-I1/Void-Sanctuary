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
                },
                {
                    ""name"": ""PauseGameAction"",
                    ""type"": ""Button"",
                    ""id"": ""278ea221-b933-45df-8e11-8c35b4a5a864"",
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
                    ""id"": ""33eca119-57a7-4334-abdf-f9344a525c97"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ExitGameAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c69804b8-2bca-47fc-9656-3f383355d10b"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseGameAction"",
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
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""e5cd0030-20f5-4aed-b3c5-bd8e762b2114"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""fb0a051d-4836-47d5-b3fe-fe457f3e9bc3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""d194de29-76dd-49ee-91a4-a63fc09d3148"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""d68017b5-7edf-4c0a-bb56-e0a9f63cc63e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DrawWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""eaddbf77-3a63-46e7-ac47-8138dc802ad1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""6f452dbe-afcb-4cff-be87-053005122fcc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""FakeHit"",
                    ""type"": ""Button"",
                    ""id"": ""d3f3c4e0-bb56-4eac-8040-8721091e2fe0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""e9262a50-cde7-46cd-b005-9ba9fead306e"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""459b546a-0064-4039-b606-ccab1fe86060"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f705e70-adde-4fce-956f-7ccb6acd7654"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5b318b34-2531-49fe-8c97-65847e874222"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93f29098-f95f-426b-b8bd-3228b5978786"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""461084db-fe98-4ed1-a650-d65ba06ed063"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efeb4a3c-41fa-47f1-b194-e455da8f2ad5"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cea987b3-ef84-4a5b-a7fe-1e07dd15f912"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1973698b-f14f-4359-8c12-4b8095b90451"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""604d7e94-9145-4175-9bfd-31513a0a14d0"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9afc636e-9702-432f-b45c-7885a4ab5cd2"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9af9bb1-ff5c-40b0-8f57-731c12560bd4"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DrawWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9482b8bc-6542-4e20-948c-bf7a5d185295"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DrawWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec8737b5-3378-4c90-845c-89963f652c1e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d789cd5b-16f6-4d06-be86-e4f63af5b65f"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4fdafc83-49f3-4080-a6d1-2a37336f6650"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FakeHit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b486fd9c-2253-40e6-ae50-a63cf8201f68"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuActionMap"",
            ""id"": ""1ac8f67b-16a8-43cf-b77e-817031acc2b2"",
            ""actions"": [
                {
                    ""name"": ""TriggerCurrentButton"",
                    ""type"": ""Button"",
                    ""id"": ""e1176676-bb22-41e3-a717-939b727a865b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""GoBackButton"",
                    ""type"": ""Button"",
                    ""id"": ""cb4d6fef-f653-4943-ae95-bdb9fdcc201f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""01339e75-5712-4be2-bded-433e439f77ba"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TriggerCurrentButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86a9c09f-7f13-4d6c-909a-c975a0a5fbd3"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TriggerCurrentButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f0d22bb1-9419-4131-9217-317c0b13cf7d"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""GoBackButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ControlsActionMap
        m_ControlsActionMap = asset.FindActionMap("ControlsActionMap", throwIfNotFound: true);
        m_ControlsActionMap_ExitGameAction = m_ControlsActionMap.FindAction("ExitGameAction", throwIfNotFound: true);
        m_ControlsActionMap_PauseGameAction = m_ControlsActionMap.FindAction("PauseGameAction", throwIfNotFound: true);
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_Fire = m_Player.FindAction("Fire", throwIfNotFound: true);
        m_Player_Block = m_Player.FindAction("Block", throwIfNotFound: true);
        m_Player_Run = m_Player.FindAction("Run", throwIfNotFound: true);
        m_Player_DrawWeapon = m_Player.FindAction("DrawWeapon", throwIfNotFound: true);
        m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
        m_Player_FakeHit = m_Player.FindAction("FakeHit", throwIfNotFound: true);
        m_Player_MousePosition = m_Player.FindAction("MousePosition", throwIfNotFound: true);
        // MenuActionMap
        m_MenuActionMap = asset.FindActionMap("MenuActionMap", throwIfNotFound: true);
        m_MenuActionMap_TriggerCurrentButton = m_MenuActionMap.FindAction("TriggerCurrentButton", throwIfNotFound: true);
        m_MenuActionMap_GoBackButton = m_MenuActionMap.FindAction("GoBackButton", throwIfNotFound: true);
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
    private readonly InputAction m_ControlsActionMap_PauseGameAction;
    public struct ControlsActionMapActions
    {
        private @VoidSanctuaryActions m_Wrapper;
        public ControlsActionMapActions(@VoidSanctuaryActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ExitGameAction => m_Wrapper.m_ControlsActionMap_ExitGameAction;
        public InputAction @PauseGameAction => m_Wrapper.m_ControlsActionMap_PauseGameAction;
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
            @PauseGameAction.started += instance.OnPauseGameAction;
            @PauseGameAction.performed += instance.OnPauseGameAction;
            @PauseGameAction.canceled += instance.OnPauseGameAction;
        }

        private void UnregisterCallbacks(IControlsActionMapActions instance)
        {
            @ExitGameAction.started -= instance.OnExitGameAction;
            @ExitGameAction.performed -= instance.OnExitGameAction;
            @ExitGameAction.canceled -= instance.OnExitGameAction;
            @PauseGameAction.started -= instance.OnPauseGameAction;
            @PauseGameAction.performed -= instance.OnPauseGameAction;
            @PauseGameAction.canceled -= instance.OnPauseGameAction;
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
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_Fire;
    private readonly InputAction m_Player_Block;
    private readonly InputAction m_Player_Run;
    private readonly InputAction m_Player_DrawWeapon;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_FakeHit;
    private readonly InputAction m_Player_MousePosition;
    public struct PlayerActions
    {
        private @VoidSanctuaryActions m_Wrapper;
        public PlayerActions(@VoidSanctuaryActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @Fire => m_Wrapper.m_Player_Fire;
        public InputAction @Block => m_Wrapper.m_Player_Block;
        public InputAction @Run => m_Wrapper.m_Player_Run;
        public InputAction @DrawWeapon => m_Wrapper.m_Player_DrawWeapon;
        public InputAction @Dash => m_Wrapper.m_Player_Dash;
        public InputAction @FakeHit => m_Wrapper.m_Player_FakeHit;
        public InputAction @MousePosition => m_Wrapper.m_Player_MousePosition;
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
            @Attack.started += instance.OnAttack;
            @Attack.performed += instance.OnAttack;
            @Attack.canceled += instance.OnAttack;
            @Fire.started += instance.OnFire;
            @Fire.performed += instance.OnFire;
            @Fire.canceled += instance.OnFire;
            @Block.started += instance.OnBlock;
            @Block.performed += instance.OnBlock;
            @Block.canceled += instance.OnBlock;
            @Run.started += instance.OnRun;
            @Run.performed += instance.OnRun;
            @Run.canceled += instance.OnRun;
            @DrawWeapon.started += instance.OnDrawWeapon;
            @DrawWeapon.performed += instance.OnDrawWeapon;
            @DrawWeapon.canceled += instance.OnDrawWeapon;
            @Dash.started += instance.OnDash;
            @Dash.performed += instance.OnDash;
            @Dash.canceled += instance.OnDash;
            @FakeHit.started += instance.OnFakeHit;
            @FakeHit.performed += instance.OnFakeHit;
            @FakeHit.canceled += instance.OnFakeHit;
            @MousePosition.started += instance.OnMousePosition;
            @MousePosition.performed += instance.OnMousePosition;
            @MousePosition.canceled += instance.OnMousePosition;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Attack.started -= instance.OnAttack;
            @Attack.performed -= instance.OnAttack;
            @Attack.canceled -= instance.OnAttack;
            @Fire.started -= instance.OnFire;
            @Fire.performed -= instance.OnFire;
            @Fire.canceled -= instance.OnFire;
            @Block.started -= instance.OnBlock;
            @Block.performed -= instance.OnBlock;
            @Block.canceled -= instance.OnBlock;
            @Run.started -= instance.OnRun;
            @Run.performed -= instance.OnRun;
            @Run.canceled -= instance.OnRun;
            @DrawWeapon.started -= instance.OnDrawWeapon;
            @DrawWeapon.performed -= instance.OnDrawWeapon;
            @DrawWeapon.canceled -= instance.OnDrawWeapon;
            @Dash.started -= instance.OnDash;
            @Dash.performed -= instance.OnDash;
            @Dash.canceled -= instance.OnDash;
            @FakeHit.started -= instance.OnFakeHit;
            @FakeHit.performed -= instance.OnFakeHit;
            @FakeHit.canceled -= instance.OnFakeHit;
            @MousePosition.started -= instance.OnMousePosition;
            @MousePosition.performed -= instance.OnMousePosition;
            @MousePosition.canceled -= instance.OnMousePosition;
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

    // MenuActionMap
    private readonly InputActionMap m_MenuActionMap;
    private List<IMenuActionMapActions> m_MenuActionMapActionsCallbackInterfaces = new List<IMenuActionMapActions>();
    private readonly InputAction m_MenuActionMap_TriggerCurrentButton;
    private readonly InputAction m_MenuActionMap_GoBackButton;
    public struct MenuActionMapActions
    {
        private @VoidSanctuaryActions m_Wrapper;
        public MenuActionMapActions(@VoidSanctuaryActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @TriggerCurrentButton => m_Wrapper.m_MenuActionMap_TriggerCurrentButton;
        public InputAction @GoBackButton => m_Wrapper.m_MenuActionMap_GoBackButton;
        public InputActionMap Get() { return m_Wrapper.m_MenuActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActionMapActions set) { return set.Get(); }
        public void AddCallbacks(IMenuActionMapActions instance)
        {
            if (instance == null || m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Add(instance);
            @TriggerCurrentButton.started += instance.OnTriggerCurrentButton;
            @TriggerCurrentButton.performed += instance.OnTriggerCurrentButton;
            @TriggerCurrentButton.canceled += instance.OnTriggerCurrentButton;
            @GoBackButton.started += instance.OnGoBackButton;
            @GoBackButton.performed += instance.OnGoBackButton;
            @GoBackButton.canceled += instance.OnGoBackButton;
        }

        private void UnregisterCallbacks(IMenuActionMapActions instance)
        {
            @TriggerCurrentButton.started -= instance.OnTriggerCurrentButton;
            @TriggerCurrentButton.performed -= instance.OnTriggerCurrentButton;
            @TriggerCurrentButton.canceled -= instance.OnTriggerCurrentButton;
            @GoBackButton.started -= instance.OnGoBackButton;
            @GoBackButton.performed -= instance.OnGoBackButton;
            @GoBackButton.canceled -= instance.OnGoBackButton;
        }

        public void RemoveCallbacks(IMenuActionMapActions instance)
        {
            if (m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMenuActionMapActions instance)
        {
            foreach (var item in m_Wrapper.m_MenuActionMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MenuActionMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MenuActionMapActions @MenuActionMap => new MenuActionMapActions(this);
    public interface IControlsActionMapActions
    {
        void OnExitGameAction(InputAction.CallbackContext context);
        void OnPauseGameAction(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnDrawWeapon(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnFakeHit(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
    public interface IMenuActionMapActions
    {
        void OnTriggerCurrentButton(InputAction.CallbackContext context);
        void OnGoBackButton(InputAction.CallbackContext context);
    }
}
