using UnityEngine;
using UnityEngine.UIElements;

public class UISoundManipulator : PointerManipulator
{

    private SoundEffectHandler m_focusSound;
    private SoundEffectHandler m_submitSound;

    public UISoundManipulator(SoundEffectHandler focusSound, SoundEffectHandler submitSound){
        m_focusSound = focusSound;
        m_submitSound = submitSound;
    }
    protected override void RegisterCallbacksOnTarget(){
        target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }
    protected override void UnregisterCallbacksFromTarget(){
        target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerEnter(PointerEnterEvent evt){
        m_focusSound.Play();
    }

    private void OnPointerDown(PointerDownEvent evt){
        m_submitSound.Play();
    }
}
