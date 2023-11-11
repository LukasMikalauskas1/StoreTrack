namespace StoreTrack.Helpers
{
	public record ResourceDto<T>(T Resources, IReadOnlyCollection<LinkDto> Links);
}
