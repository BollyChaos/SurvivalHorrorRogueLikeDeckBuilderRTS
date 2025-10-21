//using Character.Settings.RebindUI;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using UnityEngine.UI;

namespace Character.Settings
{
    public enum VALUE_TYPE { BOOL, FLOAT, STRING }
    public class UISettingsElement : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] VALUE_TYPE dataType;
        public VALUE_TYPE DataType { get { return dataType; } }
        SettingsElementEventBase eventWrapper;
        void Awake()
        {
        
        }
        public void Init()
        {
            switch (dataType)
            {
                case VALUE_TYPE.BOOL:
                    var boolEvent = new BoolSettingsElementEvent();
                    boolEvent.UIname = name;
                    eventWrapper = boolEvent;

                    GetComponent<Toggle>().onValueChanged.AddListener(boolEvent.InvokeEvent);
                    break;

                case VALUE_TYPE.FLOAT:
                    var floatEvent = new FloatSettingsElementEvent();
                    floatEvent.UIname = name;
                    eventWrapper = floatEvent;
                    GetComponent<Slider>()?.onValueChanged.AddListener(floatEvent.InvokeEvent);
                    break;

                case VALUE_TYPE.STRING:
                    // var stringEvent = new RebingSettingsElementEvent();
                    // stringEvent.UIname = name;
                    // eventWrapper = stringEvent;
                    // var rebindUI = GetComponent<RebindActionUI>();
                    // rebindUI.updateBindingUIEvent.AddListener(stringEvent.InvokeEvent); //unity hijo de puta hazlo mas facil

                    break;
            }
        }
        //public void TestTFS(RebindActionUI ui, string i, string j, string k)
        //{
        //    Debug.LogError("Binding actualizado desde el UI");
        //    Debug.LogError(ui);
        //    Debug.LogError(i);
        //    Debug.LogError(j);
        //    Debug.LogError(k);
            
        //}
        public void Subscribe<T>(UnityAction<string, T> callback)
        {

            Debug.Log($"[Subscribe] Trying to subscribe {typeof(T)} to eventWrapper of type {eventWrapper?.GetType().ToString()}");
            if (eventWrapper == null)
            {
                Debug.Log($"[Subscribe] evenWrapper es nulo");
                return;
            }
            if (eventWrapper is SettingsElementEvent<T> typedEvent)
            {
               // if (dataType == VALUE_TYPE.STRING) Debug.Log("Joooooooooooder");
                Debug.Log($"[Subscribe]Event subscribed of type: "+typeof(T));
                typedEvent.Subscribe(callback);
            }
            else
            {
                Debug.LogWarning($"Cannot subscribe: type mismatch for {typeof(T)} in {name}");
            }


        }



    }
    public abstract class SettingsElementEvent<T> : SettingsElementEventBase
    {
        public UnityEvent<string, T> onChangeSettingsElement = new UnityEvent<string, T>();
        public string UIname;
        public void InvokeEvent(T value)
        {
            //Debug.Log("[UISettingsElement] Invocando cambio de settings " + UIname + " : " + value.ToString());
            onChangeSettingsElement.Invoke(UIname, value);
        }
        public void Subscribe(UnityAction<string, T> listener)
        {
            onChangeSettingsElement.AddListener(listener);
        }

        public void Unsubscribe(UnityAction<string, T> listener)
        {
            onChangeSettingsElement.RemoveListener(listener);
        }
    }
    public class BoolSettingsElementEvent : SettingsElementEvent<bool> { }
    public class FloatSettingsElementEvent : SettingsElementEvent<float> { }
    public class StringSettingsElementEvent : SettingsElementEvent<string> { }

    public class RebingSettingsElementEvent : SettingsElementEvent<string> { 
    
    // public void InvokeEvent(RebindActionUI rebindActionUI, string binding, string controlScheme, string k)
    //     {

    //         var action = rebindActionUI.actionReference.action;

    //         string actionMap = action.actionMap?.name ?? "";
    //         string actionName = action.name;
    //         string bindingPath =$"<{controlScheme}>/{k}";

    //         string bindingData = $"{actionMap}::{actionName}::{bindingPath}";

    //         // Detectar si el binding actual es parte de un composite
    //         var bindingIndex = action.bindings.ToList<InputBinding>().FindIndex(b => b.effectivePath == bindingPath || b.path == bindingPath);
    //         bool isCompositePart = bindingIndex >= 0 && action.bindings[bindingIndex].isPartOfComposite;

          
    //         // A�adir informaci�n extra si es composite
    //         if (isCompositePart)
    //         {
    //             bindingData += $"::composite::{bindingIndex}";
    //            // Debug.LogError(bindingData);
    //         }

    //         onChangeSettingsElement.Invoke(UIname, bindingData);
    //     }
    }
    public abstract class SettingsElementEventBase { }
    
}
