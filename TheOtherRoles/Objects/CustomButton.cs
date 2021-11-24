using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace TheOtherRoles.Objects {
    public class CustomButton
    {
        public static List<CustomButton> buttons = new List<CustomButton>();
        public ActionButton actionButton;
        private ActionButton template;
        public Vector3 PositionOffset;
        public Vector3 LocalScale = Vector3.one;
        public float MaxTimer = float.MaxValue;
        public float Timer = 0f;
        private Action OnClick;
        private Action OnMeetingEnds;
        private Func<bool> HasButton;
        private Func<bool> CouldUse;
        private Action OnEffectEnds;
        public bool HasEffect;
        public bool isEffectActive = false;
        public bool showButtonText = true;
        public string buttonText = null;
        public float EffectDuration;
        public Sprite Sprite;
        public Func<Sprite> SpriteFunc;
        private bool mirror;
        private KeyCode? hotkey;
        public int Data;

        public CustomButton(
            Action OnClick,
            Func<bool> HasButton,
            Func<bool> CouldUse,
            Action OnMeetingEnds,
            Sprite Sprite,
            Vector3 PositionOffset,
            ActionButton? template,
            ActionButton? textTemplate,
            KeyCode? hotkey,
            bool HasEffect,
            float EffectDuration,
            Action OnEffectEnds,
            bool mirror = false)
        {
            this.OnClick = OnClick;
            this.HasButton = HasButton;
            this.CouldUse = CouldUse;
            this.PositionOffset = PositionOffset;
            this.OnMeetingEnds = OnMeetingEnds;
            this.HasEffect = HasEffect;
            this.EffectDuration = EffectDuration;
            this.OnEffectEnds = OnEffectEnds;
            this.Sprite = Sprite;
            this.mirror = mirror;
            this.hotkey = hotkey;
            this.template = template ?? DestroyableSingleton<HudManager>.Instance.UseButton;
            if (template)
            {
                LocalScale = template.transform.localScale;
            }

            //Timer = 16.2f;
            Timer = 10.0f;
            buttons.Add(this);
            actionButton = UnityEngine.Object.Instantiate(this.template, this.template.transform.parent);
            PassiveButton button = actionButton.GetComponent<PassiveButton>();
            button.OnClick = new Button.ButtonClickedEvent();
            button.OnClick.AddListener((UnityEngine.Events.UnityAction)onClickEvent);

            if (textTemplate)
            {
                UnityEngine.Object.Destroy(actionButton.buttonLabelText);
                actionButton.buttonLabelText = UnityEngine.Object.Instantiate(textTemplate.buttonLabelText, actionButton.transform);
            }

            setActive(false);
        }

        public CustomButton(Action OnClick, Func<bool> HasButton, Func<bool> CouldUse, Action OnMeetingEnds, Sprite Sprite, Vector3 PositionOffset, ActionButton template, ActionButton? textTemplate, KeyCode? hotkey, bool mirror = false)
        : this(OnClick, HasButton, CouldUse, OnMeetingEnds, Sprite, PositionOffset, template, textTemplate, hotkey, false, 0f, () => {}, mirror) { }

        ~CustomButton()
        {
            UnityEngine.Object.Destroy(actionButton);
        }

        void onClickEvent()
        {
            if (this.Timer < 0f && HasButton() && CouldUse())
            {
                actionButton.graphic.color = new Color(1f, 1f, 1f, 0.3f);
                this.OnClick();

                if (this.HasEffect && !this.isEffectActive) {
                    this.Timer = this.EffectDuration;
                    actionButton.cooldownTimerText.color = new Color(0F, 0.8F, 0F);
                    this.isEffectActive = true;
                }
            }
        }

        public static void HudUpdate()
        {
            buttons.RemoveAll(item => item.actionButton == null);
        
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    Helpers.log("[WARNING] NullReferenceException from HudUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        public static void MeetingEndedUpdate() {
            buttons.RemoveAll(item => item.actionButton == null);
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].OnMeetingEnds();
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    Helpers.log("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        public static void ResetAllCooldowns() {
            for (int i = 0; i < buttons.Count; i++)
            {
                try
                {
                    buttons[i].Timer = buttons[i].MaxTimer;
                    buttons[i].Update();
                }
                catch (NullReferenceException)
                {
                    Helpers.log("[WARNING] NullReferenceException from MeetingEndedUpdate().HasButton(), if theres only one warning its fine");
                }
            }
        }

        public void setActive(bool isActive) {
            if (isActive) {
                actionButton.gameObject.SetActive(true);
                actionButton.graphic.enabled = true;
            } else {
                actionButton.gameObject.SetActive(false);
                actionButton.graphic.enabled = false;
            }
        }

        private void Update()
        {
            if (actionButton == null) return;

            if (PlayerControl.LocalPlayer.Data == null || MeetingHud.Instance || ExileController.Instance || !HasButton()) {
                setActive(false);
                return;
            }
            setActive(DestroyableSingleton<HudManager>.Instance.UseButton.isActiveAndEnabled);

            if (Sprite != null)
            {
                actionButton.graphic.sprite = Sprite;
            }

            if (buttonText != null) {
                actionButton.buttonLabelText.text = buttonText;
            }
            actionButton.buttonLabelText.enabled = showButtonText; // Only show the text if it's a kill button

            if (template != null) {
                Vector3 pos = template.transform.localPosition;
                if (mirror) pos = new Vector3(-pos.x, pos.y, pos.z);
                actionButton.transform.localPosition = pos + PositionOffset;
                actionButton.transform.localScale = LocalScale;
            }

            if (CouldUse()) {
                actionButton.graphic.color = actionButton.buttonLabelText.color = Palette.EnabledColor;
                actionButton.graphic.material.SetFloat("_Desat", 0f);
            } else {
                actionButton.graphic.color = actionButton.buttonLabelText.color = Palette.DisabledClear;
                actionButton.graphic.material.SetFloat("_Desat", 1f);
            }

            if (Timer >= 0) {
                if (HasEffect && isEffectActive)
                    Timer -= Time.deltaTime;
                else if (!PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.moveable)
                    Timer -= Time.deltaTime;
            }
            
            if (Timer <= 0 && HasEffect && isEffectActive) {
                isEffectActive = false;
                actionButton.cooldownTimerText.color = Palette.EnabledColor;
                OnEffectEnds();
            }

            if (actionButton.cooldownTimerText != null)
            {
                if (HasEffect && isEffectActive)
                {
                    actionButton.SetCoolDown(Timer, EffectDuration);
                }
                else
                {
                    actionButton.SetCoolDown(Timer, MaxTimer);
                }
            }

            // Trigger OnClickEvent if the hotkey is being pressed down
            if (hotkey.HasValue && Input.GetKeyDown(hotkey.Value)) onClickEvent();
        }
    }
}