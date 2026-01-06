package com.letsgo.client;

import com.letsgo.routingclient.*;

import javax.xml.bind.JAXBElement;
import java.util.Scanner;

public class TestClient {

    private static String unwrap(JAXBElement<String> e) {
        return e != null ? e.getValue() : "";
    }

    public static void main(String[] args) {

        try {
            Scanner sc = new Scanner(System.in);

            System.out.println("=== SOAP Heavy Client for Routing Server ===");

            System.out.print("Enter origin: ");
            String origin = sc.nextLine();

            System.out.print("Enter destination: ");
            String destination = sc.nextLine();

            // Create SOAP service
            ItineraryService service = new ItineraryService();
            IItineraryService port = service.getBasicHttpBindingIItineraryService();

            System.out.println("\nRequesting itinerary...\n");

            ItineraryDto result = port.getItinerary(origin, destination);

            System.out.println("=== RESULT ===");
            System.out.println("Total km: " + result.getTotalKm());
            System.out.println("Total seconds: " + result.getTotalSeconds());

            // ---- UNWRAP Start Station ----
            StationInfo start = null;
            if (result.getStartStationDetails() != null) {
                start = result.getStartStationDetails().getValue();
            }

            // ---- UNWRAP End Station ----
            StationInfo end = null;
            if (result.getEndStationDetails() != null) {
                end = result.getEndStationDetails().getValue();
            }

            // ---- UNWRAP Note ----
            String note = "";
            if (result.getNote() != null) {
                note = result.getNote().getValue();
            }

            // ---- Print Start station ----
            if (start != null) {
                System.out.println("Start station: " + unwrap(start.getName()) +
                        " | bikes: " + start.getAvailableBikes() +
                        " | stands: " + start.getAvailableStands());
            }

            // ---- Print End station ----
            if (end != null) {
                System.out.println("End station:   " + unwrap(end.getName()) +
                        " | bikes: " + end.getAvailableBikes() +
                        " | stands: " + end.getAvailableStands());
            }

            System.out.println("Note: " + note);

        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }
}