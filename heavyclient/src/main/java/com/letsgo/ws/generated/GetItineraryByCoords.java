
package com.letsgo.ws.generated;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para anonymous complex type.
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.
 * 
 * <pre>
 * &lt;complexType&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="originLat" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="originLon" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="destLat" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *         &lt;element name="destLon" type="{http://www.w3.org/2001/XMLSchema}string" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "", propOrder = {
    "originLat",
    "originLon",
    "destLat",
    "destLon"
})
@XmlRootElement(name = "GetItineraryByCoords")
public class GetItineraryByCoords {

    @XmlElementRef(name = "originLat", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<String> originLat;
    @XmlElementRef(name = "originLon", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<String> originLon;
    @XmlElementRef(name = "destLat", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<String> destLat;
    @XmlElementRef(name = "destLon", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<String> destLon;

    /**
     * Obtiene el valor de la propiedad originLat.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getOriginLat() {
        return originLat;
    }

    /**
     * Define el valor de la propiedad originLat.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setOriginLat(JAXBElement<String> value) {
        this.originLat = value;
    }

    /**
     * Obtiene el valor de la propiedad originLon.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getOriginLon() {
        return originLon;
    }

    /**
     * Define el valor de la propiedad originLon.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setOriginLon(JAXBElement<String> value) {
        this.originLon = value;
    }

    /**
     * Obtiene el valor de la propiedad destLat.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getDestLat() {
        return destLat;
    }

    /**
     * Define el valor de la propiedad destLat.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setDestLat(JAXBElement<String> value) {
        this.destLat = value;
    }

    /**
     * Obtiene el valor de la propiedad destLon.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public JAXBElement<String> getDestLon() {
        return destLon;
    }

    /**
     * Define el valor de la propiedad destLon.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link String }{@code >}
     *     
     */
    public void setDestLon(JAXBElement<String> value) {
        this.destLon = value;
    }

}
