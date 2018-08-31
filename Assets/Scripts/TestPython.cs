using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IronPython.Hosting;
using IronPython.Modules;
using Microsoft.Scripting.Hosting;

public class TestPython : MonoBehaviour {

    public TextAsset m_textAsset;

    [SerializeField]
    private GameObject testObj;

    private ScriptScope Scope;
    private ObjectOperations Operation;

    private object classRef;

    // Use this for initialization
    void Awake () {
        // compile python
        PythonCompile();

        // create python class
        classRef = GetVariable("TestClass");

        // invoke TestClass.Awake() method
        InvokeMethod(classRef, "Awake");

        // callback test
        System.Action cb = () => {
            Debug.Log("call from python");
        };
        InvokeMethod(classRef, "CallBackTest", cb);

        Invoke("TimerStop", 10);
    }

    void Start()
    {
        //InvokeMethod(classRef, "Start");
    }

    // Update is called once per frame
    void Update () {
        //InvokeMethod(classRef, "Update");
        //InvokeMethod(classRef, "TestMethod", testObj);
    }

    private void PythonCompile()
    {
        var text = m_textAsset.text;
        var engine = Python.CreateEngine();

        Operation = engine.CreateOperations();

        engine.Runtime.LoadAssembly(typeof(GameObject).Assembly);
        engine.Runtime.LoadAssembly(typeof(Rigidbody).Assembly);

        Scope = engine.CreateScope();
        var source = engine.CreateScriptSourceFromString(text);

        source.Execute(Scope);
    }

    public void TimerStop()
    {
        InvokeMethod(classRef, "TimerStop");
    }

    // call python method
    public void InvokeMethod(object nameClass, string Method, params object[] parameters)
    {
        object output = new object();
        if (Operation.TryGetMember(nameClass, Method, out output))
        {
            object Func = Operation.GetMember(nameClass, Method);
            Operation.Invoke(Func, parameters);
        }
    }

    public object GetVariable(string name)
    {
        return Operation.Invoke(Scope.GetVariable(name));
    }

    void OnApplicationQuit()
    {
        InvokeMethod(classRef, "AbortThread");
    }
}
