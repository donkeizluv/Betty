<template>
    <span class="inline-flex">
        <span v-show="!showBefore" :class="numberColor">{{value}}</span>
        <v-icon :color="color" :class="visible? 'visible' : 'hidden'">{{iconName}}</v-icon>
        <span v-show="showBefore" :class="numberColor">{{value}}</span>
    </span>
</template>
<script>
export default {
    name: 'UpDown',
    computed:{
        visible (){
            return this.status != undefined || this.status != null;
        },
        iconName (){
            if(this.status)
                return this.upIcon;
            return this.downIcon;
        },
        color (){
            return this.status? this.upColor : this.downColor;
        },
        numberColor(){
            if(!this.visible) return this.defaultColor;
            return this.status? this.upColorNumber : this.downColorNumber;
        }
    },
    props: {
        'showBefore':{
            type: Boolean,
            default: true
        },
        'status':{
            type: Boolean,
            default: null
        },
        'value':{
            type: Number,
            required: true   
        },
        'upIcon':{
            type: String,
            default: 'arrow_drop_up'
        },
        'downIcon':{
            type: String,
            default: 'arrow_drop_down'
        },
        'upColor': {
            type: String,
            default: 'lime accent-2'
        },
        'downColor': {
            type: String,
            default: 'pink accent-2'
        },
        'upColorNumber': {
            type: String,
            default: 'lime--text text--accent-2'
        },
        'downColorNumber': {
            type: String,
            default: 'pink--text text--accent-2'
        },
        'defaultColor': {
            type: String,
            default: 'grey--text text--lighten-2'
        }
    }
}

</script>
<style scoped>
.visible{
    visibility: visible;
}
.hidden{
    visibility: hidden;
}
.inline-flex{
    display: inline-flex!important;
}
</style>