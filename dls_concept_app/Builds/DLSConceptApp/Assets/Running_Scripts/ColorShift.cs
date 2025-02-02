﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DLSConceptAppVisual
{
	public class ColorShift {

		public static GameObject Instance;
		private float[][] temp_buffer_1, temp_buffer_2, cross_buffer, switch_buffer;
		private Texture2D blackTexture, mat_texture;
		private string previous_texture, new_texture;
		private Slider slider_script;
		private GameObject slider, _camera;

		private Renderer rend;

		private List<WrapperItem> wrappers;
		private WrapperItem curr_texture_image;

		private int pixel_count;

		private bool first;
		private GUIStyle style;
		private Color last_color;
		
		public void Reset(){
			Initialize(this.rend, this.wrappers);
		}

		public ColorShift(Renderer rend, List<WrapperItem> wrappers){
			Initialize(rend, wrappers);
		}

		private void Initialize (Renderer rend, List<WrapperItem> wrappers)
		{
			this.rend = rend;
			this.wrappers = wrappers;

			slider = (slider == null) ? GameObject.Find ("Slider") : this.slider ; 
			_camera = (_camera == null) ? GameObject.Find ("CurrentTextRend") : this._camera ; 

			slider.SetActive(false);
			_camera.SetActive(false);

			slider_script =(Slider)slider.GetComponent("Slider");

			pixel_count = wrappers [0].texture.width * wrappers [0].texture.height;

			//Check this out.
			blackTexture = new Texture2D (wrappers [0].texture.width, wrappers [0].texture.height);
			mat_texture = new Texture2D (wrappers [0].texture.width, wrappers [0].texture.height);
			
			var pixel_arr = blackTexture.GetPixels();
			for (var i = 0; i < pixel_arr.Length; i++) {
				pixel_arr [i] = Color.black;
			}
			
			cross_buffer = new float[3][];
			cross_buffer[0] = new float[pixel_count];
			cross_buffer[1] = new float[pixel_count];
			cross_buffer[2] = new float[pixel_count];
			
			temp_buffer_1 = new float[3][];
			temp_buffer_1[0] = new float[pixel_count];
			temp_buffer_1[1] = new float[pixel_count];
			temp_buffer_1[2] = new float[pixel_count];

			
			temp_buffer_2 = new float[3][];
			temp_buffer_2[0] = new float[pixel_count];
			temp_buffer_2[1] = new float[pixel_count];
			temp_buffer_2[2] = new float[pixel_count];
			
			rend.material.mainTexture = BlendFirst (wrappers);
		}

		public void ChangeValue(float val){
			Color new_col = new Color (val, val, val);
			if (curr_texture_image != null) {
				rend.material.mainTexture = UpdateBlend (new_col, curr_texture_image);
			}
		}

		public void PublicColorShift(object[] tempStorage){PrivateColorShift(tempStorage);}

		private void PrivateColorShift(object[] tempStorage){
			slider.SetActive (true);
			new_texture = tempStorage [1].ToString ();
			Color color = (Color)tempStorage[0];
			var a = 0;
			for(var i = 0;i < wrappers.Count(); i++)
			{
				if(wrappers[i].texture.name == tempStorage [1].ToString ()) {a = i;}
			}
			curr_texture_image = wrappers [a];
			
			_camera.SetActive (true);
			_camera.GetComponent<Image> ().overrideSprite = wrappers [a].sprite;
			_camera.GetComponent<Image> ().color = Color.white;
			rend.material.mainTexture = UpdateBlend (color, curr_texture_image);
		}

		Texture2D UpdateBlend (Color col, WrapperItem set)
		{
			var pixel_array = mat_texture.GetPixels ();
			if (previous_texture == new_texture) {
				for (var n = 0; n < pixel_count; n++) {
					temp_buffer_1 [0] [n] = temp_buffer_2 [0] [n] - set.intensity [n] * last_color.r * last_color.a + set.intensity [n] * col.r * col.a;
					temp_buffer_1 [1] [n] = temp_buffer_2 [1] [n] - set.intensity [n] * last_color.g * last_color.a + set.intensity [n] * col.g * col.a;
					temp_buffer_1 [2] [n] = temp_buffer_2 [2] [n] - set.intensity [n] * last_color.b * last_color.a + set.intensity [n] * col.b * col.a;
					
					pixel_array [n].r = temp_buffer_1 [0] [n];
					pixel_array [n].g = temp_buffer_1 [1] [n];
					pixel_array [n].b = temp_buffer_1 [2] [n];
					
					temp_buffer_2 [0] [n] = temp_buffer_1 [0] [n];
					temp_buffer_2 [1] [n] = temp_buffer_1 [1] [n];
					temp_buffer_2 [2] [n] = temp_buffer_1 [2] [n];
				}
				set.slider = slider_script.value;
				last_color = col;
			} else {
				slider_script.value = set.slider;
				for (var n = 0; n < pixel_count; n++) {
					temp_buffer_1 [0] [n] = (temp_buffer_1 [0] [n] + set.intensity [n] * col.r * col.a);
					temp_buffer_1 [1] [n] = (temp_buffer_1 [1] [n] + set.intensity [n] * col.g * col.a);
					temp_buffer_1 [2] [n] = (temp_buffer_1 [2] [n] + set.intensity [n] * col.b * col.a);
					
					pixel_array [n].r = temp_buffer_1 [0] [n];
					pixel_array [n].g = temp_buffer_1 [1] [n];
					pixel_array [n].b = temp_buffer_1 [2] [n];
					
					temp_buffer_2 [0][n] = temp_buffer_1 [0] [n];
					temp_buffer_2 [1][n] = temp_buffer_1 [1] [n];
					temp_buffer_2 [2][n] = temp_buffer_1 [2] [n];
				}
				set.slider = slider_script.value;
				last_color = Color.black;
			}
			
			previous_texture = new_texture;
			mat_texture.SetPixels(pixel_array);
			mat_texture.Apply(true);
			return mat_texture;
		}

		Texture2D BlendFirst(List<WrapperItem> holders)
		{
			for (var i = 0; i < holders.Count; i++) {
				for (var n = 0; n < pixel_count; n++) {
					temp_buffer_1[0][n] += holders [i].intensity [n];
					temp_buffer_1[1][n] += holders [i].intensity [n];
					temp_buffer_1[2][n] += holders [i].intensity [n];
				}
			}

			var pixel_array = mat_texture.GetPixels();
			for(var n = 0; 	n < pixel_count; n++)
			{
				pixel_array[n].r = Convert.ToSingle (temp_buffer_1 [0] [n]);
				pixel_array[n].g = Convert.ToSingle (temp_buffer_1 [1] [n]);
				pixel_array[n].b = Convert.ToSingle (temp_buffer_1 [2] [n]);
				pixel_array[n].a = 1.0f;
			}

			mat_texture.SetPixels(pixel_array);
			mat_texture.Apply(true);
			return mat_texture;
		}
	}
}