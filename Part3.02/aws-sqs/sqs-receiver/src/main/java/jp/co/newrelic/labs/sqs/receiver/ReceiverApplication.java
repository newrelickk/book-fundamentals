package jp.co.newrelic.labs.sqs.receiver;

import com.amazonaws.regions.Regions;
import com.amazonaws.services.sqs.AmazonSQS;
import com.amazonaws.services.sqs.AmazonSQSClientBuilder;
import com.amazonaws.services.sqs.model.Message;
import com.amazonaws.services.sqs.model.ReceiveMessageRequest;
import com.newrelic.api.agent.Insights;
import com.newrelic.api.agent.NewRelic;
import com.newrelic.api.agent.Trace;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.ApplicationContext;
import org.springframework.context.annotation.Bean;

import java.util.Arrays;
import java.util.List;

@SpringBootApplication
public class ReceiverApplication {

    public static void main(String[] args) {
        SpringApplication.run(ReceiverApplication.class, args);
    }

    @Bean
    public CommandLineRunner commandLineRunner(ApplicationContext ctx) {
        return args -> {

            Insights insights = NewRelic.getAgent().getInsights();

            System.out.println("Starting to listen message for " + System.getenv("SQS_URL") + " :");

            AmazonSQS sqs = AmazonSQSClientBuilder.standard()
                    .withRegion(Regions.AP_NORTHEAST_1)
                    .build();
            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(System.getenv("SQS_URL"))
                    .withMaxNumberOfMessages(1)
                    .withMessageAttributeNames("TraceContext");

            while (true) {
                List<Message> messages = sqs.receiveMessage(receiveMessageRequest).getMessages();
                for (Message message:
                     messages) {
                    run(message);
                    sqs.deleteMessage(System.getenv("SQS_URL"), message.getReceiptHandle());
                }
                Thread.sleep(3000);
            }
        };
    }

    @Trace(dispatcher = true)
    public void run(Message message) {
        System.out.println("processing a message...");
        NewRelic.setTransactionName("Background", "/ReceiverApplication/run");
        NewRelic.addCustomParameter("SQSMessageId", message.getMessageId());
        System.out.println(message.getMessageAttributes().get("TraceContext").getStringValue());
        NewRelic.getAgent().getTransaction().acceptDistributedTracePayload(message.getMessageAttributes().get("TraceContext").getStringValue());
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
        System.out.println("Completed.");
    }
}
