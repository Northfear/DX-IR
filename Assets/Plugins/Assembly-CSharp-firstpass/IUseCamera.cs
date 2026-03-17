using UnityEngine;

public interface IUseCamera
{
	Camera RenderCamera { get; set; }

	void SetCamera();

	void SetCamera(Camera c);
}
