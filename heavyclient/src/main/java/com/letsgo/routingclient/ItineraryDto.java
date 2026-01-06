
package com.letsgo.routingclient;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElement;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Java class for ItineraryDto complex type.
 * 
 * <p>The following schema fragment specifies the expected content contained within this class.
 * 
 * <pre>
 * &lt;complexType name="ItineraryDto"&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="BikeMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestLat" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestLon" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="DestinationCity" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="EndStation" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="EndStationDetails" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}StationInfo" minOccurs="0"/&gt;
 *         &lt;element name="IsInterCity" type="{http://www.w3.org/2001/XMLSchema}boolean" minOccurs="0"/&gt;
 *         &lt;element name="IsWalkingOnly" type="{http://www.w3.org/2001/XMLSchema}boolean" minOccurs="0"/&gt;
 *         &lt;element name="Note" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="OriginCity" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="OriginLat" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="OriginLon" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="StartStation" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="StartStationDetails" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}StationInfo" minOccurs="0"/&gt;
 *         &lt;element name="TotalKm" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="TotalSeconds" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="WalkToEndMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *         &lt;element name="WalkToStartMeters" type="{http://www.w3.org/2001/XMLSchema}double" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "ItineraryDto", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", propOrder = {
    "bikeMeters",
    "destLat",
    "destLon",
    "destinationCity",
    "endStation",
    "endStationDetails",
    "isInterCity",
    "isWalkingOnly",
    "note",
    "originCity",
    "originLat",
    "originLon",
    "startStation",
    "startStationDetails",
    "totalKm",
    "totalSeconds",
    "walkToEndMeters",
    "walkToStartMeters"
})
public class ItineraryDto {

    @XmlElement(name = "BikeMeters")
    protected Double bikeMeters;
    @XmlElement(name = "DestLat")
    protected Double destLat;
    @XmlElement(name = "DestLon")
    protected Double destLon;
    @XmlElementRef(name = "DestinationCity", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> destinationCity;
    @XmlElementRef(name = "EndStation", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> endStation;
    @XmlElementRef(name = "EndStationDetails", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<StationInfo> endStationDetails;
    @XmlElement(name = "IsInterCity")
    protected Boolean isInterCity;
    @XmlElement(name = "IsWalkingOnly")
    protected Boolean isWalkingOnly;
    @XmlElementRef(name = "Note", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> note;
    @XmlElementRef(name = "OriginCity", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> originCity;
    @XmlElement(name = "OriginLat")
    protected Double originLat;
    @XmlElement(name = "OriginLon")
    protected Double originLon;
    @XmlElementRef(name = "StartStation", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<String> startStation;
    @XmlElementRef(name = "StartStationDetails", namespace = "http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared", type = JAXBElement.class, required = false)
    protected JAXBElement<StationInfo> startStationDetails;
    @XmlElement(name = "TotalKm")
    protected Double totalKm;
    @XmlElement(name = "TotalSeconds")
    protected Double totalSeconds;
    @XmlElement(name = "WalkToEndMeters")
    protected Double walkToEndMeters;
    @XmlElement(name = "WalkToStartMeters")
    protected Double walkToStartMeters;

    /**
     * Gets the value of the bikeMeters property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getBikeMeters() {
        return bikeMeters;
    }

    /**
     * Sets the value of the bikeMeters property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setBikeMeters(Double value) {
        this.bikeMeters = value;
    }

    /**
     * Gets the value of the destLat property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getDestLat() {
        return destLat;
    }

    /**
     * Sets the value of the destLat property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setDestLat(Double value) {
        this.destLat = value;
    }

    /**
     * Gets the value of the destLon property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getDestLon() {
        return destLon;
    }

    /**
     * Sets the value of the destLon property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setDestLon(Double value) {
        this.destLon = value;
    }

    /**
     * Gets the value of the destinationCity property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getDestinationCity() {
        return destinationCity;
    }

    /**
     * Sets the value of the destinationCity property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setDestinationCity(JAXBElement<String> value) {
        this.destinationCity = value;
    }

    /**
     * Gets the value of the endStation property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getEndStation() {
        return endStation;
    }

    /**
     * Sets the value of the endStation property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setEndStation(JAXBElement<String> value) {
        this.endStation = value;
    }

    /**
     * Gets the value of the endStationDetails property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public JAXBElement<StationInfo> getEndStationDetails() {
        return endStationDetails;
    }

    /**
     * Sets the value of the endStationDetails property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public void setEndStationDetails(JAXBElement<StationInfo> value) {
        this.endStationDetails = value;
    }

    /**
     * Gets the value of the isInterCity property.
     * 
     * @return
     *     possible object is
     *     {@link Boolean }
     *     
     */
    public Boolean isIsInterCity() {
        return isInterCity;
    }

    /**
     * Sets the value of the isInterCity property.
     * 
     * @param value
     *     allowed object is
     *     {@link Boolean }
     *     
     */
    public void setIsInterCity(Boolean value) {
        this.isInterCity = value;
    }

    /**
     * Gets the value of the isWalkingOnly property.
     * 
     * @return
     *     possible object is
     *     {@link Boolean }
     *     
     */
    public Boolean isIsWalkingOnly() {
        return isWalkingOnly;
    }

    /**
     * Sets the value of the isWalkingOnly property.
     * 
     * @param value
     *     allowed object is
     *     {@link Boolean }
     *     
     */
    public void setIsWalkingOnly(Boolean value) {
        this.isWalkingOnly = value;
    }

    /**
     * Gets the value of the note property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getNote() {
        return note;
    }

    /**
     * Sets the value of the note property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setNote(JAXBElement<String> value) {
        this.note = value;
    }

    /**
     * Gets the value of the originCity property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getOriginCity() {
        return originCity;
    }

    /**
     * Sets the value of the originCity property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setOriginCity(JAXBElement<String> value) {
        this.originCity = value;
    }

    /**
     * Gets the value of the originLat property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getOriginLat() {
        return originLat;
    }

    /**
     * Sets the value of the originLat property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setOriginLat(Double value) {
        this.originLat = value;
    }

    /**
     * Gets the value of the originLon property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getOriginLon() {
        return originLon;
    }

    /**
     * Sets the value of the originLon property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setOriginLon(Double value) {
        this.originLon = value;
    }

    /**
     * Gets the value of the startStation property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getStartStation() {
        return startStation;
    }

    /**
     * Sets the value of the startStation property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setStartStation(JAXBElement<String> value) {
        this.startStation = value;
    }

    /**
     * Gets the value of the startStationDetails property.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public JAXBElement<StationInfo> getStartStationDetails() {
        return startStationDetails;
    }

    /**
     * Sets the value of the startStationDetails property.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link StationInfo }{@code >}
     *     
     */
    public void setStartStationDetails(JAXBElement<StationInfo> value) {
        this.startStationDetails = value;
    }

    /**
     * Gets the value of the totalKm property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getTotalKm() {
        return totalKm;
    }

    /**
     * Sets the value of the totalKm property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setTotalKm(Double value) {
        this.totalKm = value;
    }

    /**
     * Gets the value of the totalSeconds property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getTotalSeconds() {
        return totalSeconds;
    }

    /**
     * Sets the value of the totalSeconds property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setTotalSeconds(Double value) {
        this.totalSeconds = value;
    }

    /**
     * Gets the value of the walkToEndMeters property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getWalkToEndMeters() {
        return walkToEndMeters;
    }

    /**
     * Sets the value of the walkToEndMeters property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setWalkToEndMeters(Double value) {
        this.walkToEndMeters = value;
    }

    /**
     * Gets the value of the walkToStartMeters property.
     * 
     * @return
     *     possible object is
     *     {@link Double }
     *     
     */
    public Double getWalkToStartMeters() {
        return walkToStartMeters;
    }

    /**
     * Sets the value of the walkToStartMeters property.
     * 
     * @param value
     *     allowed object is
     *     {@link Double }
     *     
     */
    public void setWalkToStartMeters(Double value) {
        this.walkToStartMeters = value;
    }

}
