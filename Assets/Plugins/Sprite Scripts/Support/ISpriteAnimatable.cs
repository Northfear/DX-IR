public interface ISpriteAnimatable
{
	ISpriteAnimatable prev { get; set; }

	ISpriteAnimatable next { get; set; }

	bool StepAnim(float time);
}
