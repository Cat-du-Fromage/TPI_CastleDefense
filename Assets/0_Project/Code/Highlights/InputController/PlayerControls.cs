//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/0_Project/Code/Highlights/InputController/PlayerControls.inputactions
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

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d9886de1-e17e-4b51-9d9a-6328fd4a235f"",
            ""actions"": [
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""PassThrough"",
                    ""id"": ""914dfaba-2b11-4840-8092-2482efbab5df"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Value"",
                    ""id"": ""813e5d1a-8266-43e4-ac20-cf55412871de"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftMouseClickAndMove"",
                    ""type"": ""Value"",
                    ""id"": ""6289a388-b327-4c32-8794-f08f3b11c5a3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""63c0e504-4469-4fab-b95f-97380e1a91ba"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""PositionAt"",
                    ""id"": ""9e0f2f32-8e72-4d73-8625-6d9071d8111e"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""74eb2836-cc06-4702-acbb-09985e34e460"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""58440a82-9e62-4c6e-a648-49f40e433946"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""StartPosition"",
                    ""id"": ""fd570852-0be0-4ec3-b7d1-753304d1226f"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""0b9fe2e9-79b3-437e-84ed-1d9e34cdbde9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""6de52760-4dad-4111-8281-415fa22bfab1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Placement"",
            ""id"": ""003142fe-1f39-4629-8685-060c1c8e74b7"",
            ""actions"": [
                {
                    ""name"": ""RightMouseClickAndMove"",
                    ""type"": ""Value"",
                    ""id"": ""d32fb7de-7e1c-42e9-9e6c-ad3679670f8c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SpaceKey"",
                    ""type"": ""Button"",
                    ""id"": ""1bc57889-f4c6-4152-9195-c755ab2d7615"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Placement"",
                    ""id"": ""912a1aba-046d-45bb-9a56-3f53d537542d"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseClickAndMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f1deddef-2b91-4008-8d28-8848a393bbf2"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""dbaae1af-f839-475a-9d59-e49ebb0c63d9"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""80136c70-d599-49b4-9e62-fbaa17168c14"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpaceKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Preselection"",
            ""id"": ""1e577cc4-e689-41a0-a0b9-415299fceaba"",
            ""actions"": [
                {
                    ""name"": ""MouseMove"",
                    ""type"": ""PassThrough"",
                    ""id"": ""00707b96-636b-4164-a0b8-ea068bc79277"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftMouseClickAndMove"",
                    ""type"": ""Value"",
                    ""id"": ""dd6e0d1d-9ad4-4380-af29-8faaac9edddf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""094885c4-80ef-401c-91ae-8882c846ff01"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""StartPosition"",
                    ""id"": ""321882df-bc12-4673-a7bd-43fd0ca0f2b8"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""17b28ba8-6f8f-45bb-9d6e-9f8771b0885c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""1d56e3e3-e479-4289-a620-c7729012974e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClickAndMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Selection"",
            ""id"": ""92b904e8-ef40-4130-a3a1-81e3b11f8fc7"",
            ""actions"": [
                {
                    ""name"": ""LeftMouseClick"",
                    ""type"": ""Value"",
                    ""id"": ""0c1716fa-6f6b-47fd-a6d8-188bd0f5ffdc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""PositionAt"",
                    ""id"": ""819156fb-1787-4209-942b-a53112515963"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f1f062e6-0c41-4575-8f83-d043e35fa03f"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""98ecc0ca-92e6-4c51-8791-3702b6e8fc42"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_MouseMove = m_Player.FindAction("MouseMove", throwIfNotFound: true);
        m_Player_LeftMouseClick = m_Player.FindAction("LeftMouseClick", throwIfNotFound: true);
        m_Player_LeftMouseClickAndMove = m_Player.FindAction("LeftMouseClickAndMove", throwIfNotFound: true);
        // Placement
        m_Placement = asset.FindActionMap("Placement", throwIfNotFound: true);
        m_Placement_RightMouseClickAndMove = m_Placement.FindAction("RightMouseClickAndMove", throwIfNotFound: true);
        m_Placement_SpaceKey = m_Placement.FindAction("SpaceKey", throwIfNotFound: true);
        // Preselection
        m_Preselection = asset.FindActionMap("Preselection", throwIfNotFound: true);
        m_Preselection_MouseMove = m_Preselection.FindAction("MouseMove", throwIfNotFound: true);
        m_Preselection_LeftMouseClickAndMove = m_Preselection.FindAction("LeftMouseClickAndMove", throwIfNotFound: true);
        // Selection
        m_Selection = asset.FindActionMap("Selection", throwIfNotFound: true);
        m_Selection_LeftMouseClick = m_Selection.FindAction("LeftMouseClick", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_MouseMove;
    private readonly InputAction m_Player_LeftMouseClick;
    private readonly InputAction m_Player_LeftMouseClickAndMove;
    public struct PlayerActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseMove => m_Wrapper.m_Player_MouseMove;
        public InputAction @LeftMouseClick => m_Wrapper.m_Player_LeftMouseClick;
        public InputAction @LeftMouseClickAndMove => m_Wrapper.m_Player_LeftMouseClickAndMove;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @MouseMove.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseMove;
                @MouseMove.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseMove;
                @MouseMove.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseMove;
                @LeftMouseClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClickAndMove.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftMouseClickAndMove;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
                @LeftMouseClickAndMove.started += instance.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.performed += instance.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.canceled += instance.OnLeftMouseClickAndMove;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Placement
    private readonly InputActionMap m_Placement;
    private IPlacementActions m_PlacementActionsCallbackInterface;
    private readonly InputAction m_Placement_RightMouseClickAndMove;
    private readonly InputAction m_Placement_SpaceKey;
    public struct PlacementActions
    {
        private @PlayerControls m_Wrapper;
        public PlacementActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @RightMouseClickAndMove => m_Wrapper.m_Placement_RightMouseClickAndMove;
        public InputAction @SpaceKey => m_Wrapper.m_Placement_SpaceKey;
        public InputActionMap Get() { return m_Wrapper.m_Placement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlacementActions set) { return set.Get(); }
        public void SetCallbacks(IPlacementActions instance)
        {
            if (m_Wrapper.m_PlacementActionsCallbackInterface != null)
            {
                @RightMouseClickAndMove.started -= m_Wrapper.m_PlacementActionsCallbackInterface.OnRightMouseClickAndMove;
                @RightMouseClickAndMove.performed -= m_Wrapper.m_PlacementActionsCallbackInterface.OnRightMouseClickAndMove;
                @RightMouseClickAndMove.canceled -= m_Wrapper.m_PlacementActionsCallbackInterface.OnRightMouseClickAndMove;
                @SpaceKey.started -= m_Wrapper.m_PlacementActionsCallbackInterface.OnSpaceKey;
                @SpaceKey.performed -= m_Wrapper.m_PlacementActionsCallbackInterface.OnSpaceKey;
                @SpaceKey.canceled -= m_Wrapper.m_PlacementActionsCallbackInterface.OnSpaceKey;
            }
            m_Wrapper.m_PlacementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @RightMouseClickAndMove.started += instance.OnRightMouseClickAndMove;
                @RightMouseClickAndMove.performed += instance.OnRightMouseClickAndMove;
                @RightMouseClickAndMove.canceled += instance.OnRightMouseClickAndMove;
                @SpaceKey.started += instance.OnSpaceKey;
                @SpaceKey.performed += instance.OnSpaceKey;
                @SpaceKey.canceled += instance.OnSpaceKey;
            }
        }
    }
    public PlacementActions @Placement => new PlacementActions(this);

    // Preselection
    private readonly InputActionMap m_Preselection;
    private IPreselectionActions m_PreselectionActionsCallbackInterface;
    private readonly InputAction m_Preselection_MouseMove;
    private readonly InputAction m_Preselection_LeftMouseClickAndMove;
    public struct PreselectionActions
    {
        private @PlayerControls m_Wrapper;
        public PreselectionActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseMove => m_Wrapper.m_Preselection_MouseMove;
        public InputAction @LeftMouseClickAndMove => m_Wrapper.m_Preselection_LeftMouseClickAndMove;
        public InputActionMap Get() { return m_Wrapper.m_Preselection; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PreselectionActions set) { return set.Get(); }
        public void SetCallbacks(IPreselectionActions instance)
        {
            if (m_Wrapper.m_PreselectionActionsCallbackInterface != null)
            {
                @MouseMove.started -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnMouseMove;
                @MouseMove.performed -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnMouseMove;
                @MouseMove.canceled -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnMouseMove;
                @LeftMouseClickAndMove.started -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.performed -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.canceled -= m_Wrapper.m_PreselectionActionsCallbackInterface.OnLeftMouseClickAndMove;
            }
            m_Wrapper.m_PreselectionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseMove.started += instance.OnMouseMove;
                @MouseMove.performed += instance.OnMouseMove;
                @MouseMove.canceled += instance.OnMouseMove;
                @LeftMouseClickAndMove.started += instance.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.performed += instance.OnLeftMouseClickAndMove;
                @LeftMouseClickAndMove.canceled += instance.OnLeftMouseClickAndMove;
            }
        }
    }
    public PreselectionActions @Preselection => new PreselectionActions(this);

    // Selection
    private readonly InputActionMap m_Selection;
    private ISelectionActions m_SelectionActionsCallbackInterface;
    private readonly InputAction m_Selection_LeftMouseClick;
    public struct SelectionActions
    {
        private @PlayerControls m_Wrapper;
        public SelectionActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMouseClick => m_Wrapper.m_Selection_LeftMouseClick;
        public InputActionMap Get() { return m_Wrapper.m_Selection; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SelectionActions set) { return set.Get(); }
        public void SetCallbacks(ISelectionActions instance)
        {
            if (m_Wrapper.m_SelectionActionsCallbackInterface != null)
            {
                @LeftMouseClick.started -= m_Wrapper.m_SelectionActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.performed -= m_Wrapper.m_SelectionActionsCallbackInterface.OnLeftMouseClick;
                @LeftMouseClick.canceled -= m_Wrapper.m_SelectionActionsCallbackInterface.OnLeftMouseClick;
            }
            m_Wrapper.m_SelectionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftMouseClick.started += instance.OnLeftMouseClick;
                @LeftMouseClick.performed += instance.OnLeftMouseClick;
                @LeftMouseClick.canceled += instance.OnLeftMouseClick;
            }
        }
    }
    public SelectionActions @Selection => new SelectionActions(this);
    public interface IPlayerActions
    {
        void OnMouseMove(InputAction.CallbackContext context);
        void OnLeftMouseClick(InputAction.CallbackContext context);
        void OnLeftMouseClickAndMove(InputAction.CallbackContext context);
    }
    public interface IPlacementActions
    {
        void OnRightMouseClickAndMove(InputAction.CallbackContext context);
        void OnSpaceKey(InputAction.CallbackContext context);
    }
    public interface IPreselectionActions
    {
        void OnMouseMove(InputAction.CallbackContext context);
        void OnLeftMouseClickAndMove(InputAction.CallbackContext context);
    }
    public interface ISelectionActions
    {
        void OnLeftMouseClick(InputAction.CallbackContext context);
    }
}
