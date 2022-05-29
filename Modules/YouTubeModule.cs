using CliWrap;
using Discord.Audio;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

public class YouTubeModule : ModuleBase<SocketCommandContext>
{
    // ~say hello world -> hello world
    [Command("play")]
    [Summary("Play youTube music.")]
    public async Task PlayAsync([Remainder][Summary("YouTube url")] string url)
    {
        YoutubeClient youtube = new YoutubeClient();
        var StreamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
        var StreamInfo = StreamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        var stream = await youtube.Videos.Streams.GetAsync(StreamInfo);

        var memoryStream = new MemoryStream();
        await Cli.Wrap("ffmpeg")
            .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
            .WithStandardInputPipe(PipeSource.FromStream(stream))
            .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
            .ExecuteAsync();
        
        //using(var discord = AudioClient.CreatePCMStream(AudioApplication.Mixed))
        //{
        //    try { await AudioClient.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length); }
        //    finally { await AudioClient.FlushAsync(); }
        //}
    }

    // ReplyAsync is a method on ModuleBase 
}

