package jp.co.newrelic.labs.sqs.sender;

import com.amazonaws.regions.Regions;
import com.amazonaws.services.sqs.AmazonSQS;
import com.amazonaws.services.sqs.AmazonSQSClientBuilder;
import com.amazonaws.services.sqs.model.MessageAttributeValue;
import com.amazonaws.services.sqs.model.SendMessageRequest;
import com.amazonaws.services.sqs.model.SendMessageResult;
import com.newrelic.api.agent.DistributedTracePayload;
import com.newrelic.api.agent.NewRelic;
import com.newrelic.api.agent.Segment;
import com.newrelic.api.agent.Transaction;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.HashMap;
import java.util.Map;

@RestController
public class Sender {
    @RequestMapping("/send")
    public String send() {

        var sb = new StringBuilder();

        var sqs = AmazonSQSClientBuilder.standard()
                .withRegion(Regions.AP_NORTHEAST_1)
                .build();

        var txn = NewRelic.getAgent().getTransaction();
        var dtp = txn.createDistributedTracePayload();
        var traceContext = dtp.text();
        var messageAttributes = new HashMap<String, MessageAttributeValue>();
        messageAttributes.put("TraceContext", new MessageAttributeValue()
                .withDataType("String")
                .withStringValue(traceContext));

        System.out.println("TraceContext: "+ traceContext);

        sb.append("<p>Trace Context: ").append(traceContext);

        System.out.println(System.getenv("SQS_URL"));
        var message = new SendMessageRequest()
                .withQueueUrl(System.getenv("SQS_URL"))
                .withMessageBody("hello world")
                .withMessageAttributes(messageAttributes)
                .withDelaySeconds(5);

        var segment = NewRelic.getAgent().getTransaction().startSegment("External", "SQS");
        var result = sqs.sendMessage(message);
        segment.end();
        return sb.append("</p><p>Created message: ").append(result.getMessageId()).append("</p>").toString();
    }
}
