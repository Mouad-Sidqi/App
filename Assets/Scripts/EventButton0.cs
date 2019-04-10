using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventButton0 : MonoBehaviour {
//#5db827
	public GameObject Menu0;
	public GameObject Menu1;
	public GameObject Menu2;
	public GameObject Menu3;
	public GameObject ExtraContainer;
	public GameObject buttonsContainer;
	public GameObject whiteBackground;


		public void buttonEvent0() {
		// if (infodiv.activeSelf)
		// 	infodiv.SetActive(false);
		// else {
			Menu0.SetActive(false);
			Menu1.SetActive(false);
			// Menu2.SetActive(false);
			// Menu3.SetActive(false);
			ExtraContainer.SetActive(false);
			//buttonsContainer.SetActive(false);
			whiteBackground.SetActive(false);
	
	}	

		public void buttonEvent1() {

			Menu0.SetActive(true);
			// Menu1.SetActive(false);
			// Menu2.SetActive(false);
			// Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			// buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
	
	}	
		public void buttonEvent2() {

			Menu0.SetActive(false);
			Menu1.SetActive(true);
			// Menu2.SetActive(false);
			// Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			//buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
			
	}	
		public void buttonEvent3() {

			Menu0.SetActive(false);
			Menu1.SetActive(false);
			Menu2.SetActive(true);
			Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);

	}	
		public void buttonEvent4() {

			Menu0.SetActive(false);
			Menu1.SetActive(false);
			Menu2.SetActive(false);
			Menu3.SetActive(true);
			ExtraContainer.SetActive(true);
			buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
	
	}	

	public void buttonEvent5() {

			Menu0.SetActive(false);
			Menu1.SetActive(false);
			Menu2.SetActive(false);
			Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
	
	}	
		public void buttonEvent6() {

			Menu0.SetActive(false);
			Menu1.SetActive(false);
			Menu2.SetActive(false);
			Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
	
	}	
		public void buttonEvent7() {

			Menu0.SetActive(false);
			Menu1.SetActive(false);
			Menu2.SetActive(false);
			Menu3.SetActive(false);
			ExtraContainer.SetActive(true);
			buttonsContainer.SetActive(true);
			whiteBackground.SetActive(true);
	
	}	
}
