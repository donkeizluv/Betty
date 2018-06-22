<template>
    <v-layout align-center row>
        <v-flex v-show="showPrefix" sm2 xs2>
            <span class="caption prefix">
                {{prefix}}
            </span>
        </v-flex>
        <v-layout column>
            <v-flex sm1 xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', numberClass]">{{days}}</span>
                <span class="text">Ngày</span>
            </v-flex>
        </v-layout>
        <v-layout column>
            <v-flex sm1 xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', numberClass]">{{hours}}</span>
                <span class="text">Giờ</span>
            </v-flex>
        </v-layout>
        <v-layout column>
            <v-flex sm1 xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', numberClass]">{{minutes}}</span>
                <span class="text">Phút</span>
            </v-flex>
        </v-layout>
        <v-layout column>
            <v-flex sm1 xs1 wrap>
                <span :class="[timedOut ? 'red-digit' : 'digit', numberClass]">{{seconds}}</span>
                <span class="text">Giây</span>
            </v-flex>
        </v-layout>
        
    </v-layout>
</template>
<script>
import parse from 'date-fns/parse'
export default {
    created () {
        if(this.to){
            this.toTick = Math.trunc(parse(this.to) / 1000);
        }
        else{
            this.toTick = 0;
        }
        this.tick(); //First tick
        this.timerId = window.setInterval(this.tick, 1000);
    },
    props : {
        from: {
            type: String,
            required: true
        },
        to : {
            type: String,
            required: true
        },
        prefix: {
            type: String,
            default: null
        },
        numberClass: {
            type: String,
            default: "headline"
        }
    },
    data() {
        return {
            fromTick: Math.trunc(parse(this.from).getTime() / 1000),
            toTick: null,
            timerId: null,
            timedOut: false
        }
    },
    methods: {
        tick(){
            if(this.fromTick >= this.toTick){
                this.$emit('timedout');
                this.timedOut = true;
                if(this.timerId)
                    window.clearInterval(this.timerId);
                return;
            }
            this.fromTick++;
        },
        twoDigit(value){
            if(!value) return "00";
            if(value.toString().length <= 1) {
                return "0"+value.toString();
            }
            return value.toString();
        }
    },
    computed: {
        showPrefix(){
            return !!this.prefix;
        },
        seconds() {
            return this.twoDigit(this.timedOut? 0 : (this.toTick - this.fromTick) % 60);
        },
        minutes() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.toTick - this.fromTick) / 60) % 60);
        },
        hours() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.toTick - this.fromTick) / 60 / 60) % 24);
        },
        days() {
            return this.twoDigit(this.timedOut? 0 : Math.trunc((this.toTick - this.fromTick) / 60 / 60 / 24));
        }
    }
}
</script>
<style scoped>
.text {
    color: #5dc596;
    font-weight: 400;
    text-align: center;
}
.digit {
    color: #E0E0E0;
    font-weight: 100;
    text-align: center;
}
.red-digit {
    color: #EF5350;
    font-weight: 100;
    text-align: center;
}
.prefix{
    color: #E0E0E0;
}
</style>