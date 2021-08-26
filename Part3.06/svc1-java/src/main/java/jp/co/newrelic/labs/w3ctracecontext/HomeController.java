package jp.co.newrelic.labs.w3ctracecontext;

import org.springframework.http.MediaType;
import org.springframework.http.client.reactive.ReactorClientHttpConnector;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.reactive.function.client.WebClient;
import reactor.core.publisher.Mono;

import java.io.IOException;
//標準HttpClient用
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

@RestController
public class HomeController {
    @RequestMapping("/")
    public Mono<String> index() throws IOException, InterruptedException {
        var url = System.getenv("SVC2_URL") + "/hello";
        var webClient = WebClient.builder().baseUrl(System.getenv("SVC2_URL")).build();
        System.out.println("sending request...");
        var resp = webClient.get().uri("/hello").retrieve().bodyToMono(String.class);
        return resp;
    }
}
